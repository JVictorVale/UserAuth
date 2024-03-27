using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Core.Interfaces;
using UserAuth.Application.Contracts;
using UserAuth.Application.DTOs.Auth;
using UserAuth.Application.DTOs.Usuario;
using UserAuth.Application.Email;
using UserAuth.Application.Notifications;
using UserAuth.Core.Enums;
using UserAuth.Core.Settings;
using UserAuth.Domain.Contracts.Repositories;
using UserAuth.Domain.Entities;

namespace UserAuth.Application.Services;

public class AuthService : BaseService, IAuthService
{
    private readonly IFileService _fileService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;
    private readonly IEmailService _emailService;
    
    public AuthService(IMapper mapper, INotificator notificator, IUsuarioRepository usuarioRepository, IPasswordHasher<Usuario> passwordHasher, IJwtService jwtService, IOptions<JwtSettings> jwtSettings, IEmailService emailService, IFileService fileService) : base(mapper, notificator)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _emailService = emailService;
        _fileService = fileService;
        _jwtSettings = jwtSettings.Value;
    }
    
    public async Task<UsuarioDto?> Cadastrar(RegistrarUsuarioDto registrarUsuarioDto)
    {
        var usuario = Mapper.Map<Usuario>(registrarUsuarioDto);
        
        if (registrarUsuarioDto.Senha != registrarUsuarioDto.ConfirmarSenha)
        {
            Notificator.Handle("As senhas informadas não coincidem");
            return null;
        }
        
        if (!await Validar(usuario))
        {
            return null;
        }
        
        if (registrarUsuarioDto.Foto is { Length: > 0 })
        {
            usuario.Foto = await _fileService.Upload(registrarUsuarioDto.Foto, EUploadPath.FotoUsuarios);
        }
        
        usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);
        
        usuario.TokenDeVerificacao = CreateRandomToken();
        
        _usuarioRepository.Cadastrar(usuario);
        if (await _usuarioRepository.UnitOfWork.Commit())
        {
            await _emailService.SendEmailVerification(usuario);
            
            return Mapper.Map<UsuarioDto?>(usuario);
        }

        Notificator.Handle("Não foi possível cadastrar o usuário");
        return null;
    }
    
    public async Task<TokenDto?> Login(LoginUsuarioDto loginUsuarioDto)
    {
        var usuario = await _usuarioRepository.ObterPorEmail(loginUsuarioDto.Email);
        if (usuario == null)
        {
            return null;
        }
        
        if (!usuario.ContaVerificada)
        {
            Notificator.Handle("O usuário não está verificado.");
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, loginUsuarioDto.Senha);
        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return new TokenDto
        {
            Token = await GerarToken(usuario)
        };
    }
    
    
    public async Task<bool> VerificarEmail(string token, string email)
    {
        var usuario = await _usuarioRepository.ObterPorEmail(email);

        if (usuario == null || usuario.TokenDeVerificacao != token)
        {
            Notificator.Handle("Token de verificação inválido");
            return false;
        }

        usuario.VerificadoEm = DateTime.Now;
        usuario.ContaVerificada = true;
        _usuarioRepository.Atualizar(usuario);

        if (await _usuarioRepository.UnitOfWork.Commit())
        {
            return true;
        }

        Notificator.Handle("Não foi possível verificar o e-mail");
        return false;
    }
    
    public async Task<bool> EsqueceuSenha(string email)
    {
        var usuario = await _usuarioRepository.ObterPorEmail(email);
        if (usuario == null)
        {
            Notificator.Handle("Usuário não encontrado.");
            return false;
        }
        
        var pedidoResetSenha = await _usuarioRepository.ObterPedidoResetSenhaValido(usuario.Id);
        if (pedidoResetSenha != null)
        {
            Notificator.Handle("Já existe um pedido de recuperação de senha em andamento para este usuário.");
            return false;
        }
    
        usuario.TokenDeResetSenha = CriarTokenEsqueceuSenha();
        usuario.ExpiraResetToken = DateTime.Now.AddHours(3);
        _usuarioRepository.Atualizar(usuario);
        
        await _usuarioRepository.UnitOfWork.Commit();
    
        await _emailService.SendEmailRecoverPassword(usuario);
    
        return true;
    }
    
    public async Task<bool> ResetSenha(ResetSenhaDto requestDto)
    {
        var usuario = await _usuarioRepository.ObterPorTokenDeResetSenha(requestDto.Token);
        if (usuario == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }

        if (usuario.ExpiraResetToken.HasValue && usuario.ExpiraResetToken.Value < DateTime.Now)
        {
            Notificator.Handle("O token de redefinição de senha expirou.");
            return false;
        }
        
        if (!string.IsNullOrEmpty(requestDto.Senha) && requestDto.Senha != requestDto.ConfirmarSenha)
        {
            Notificator.Handle("As senhas informadas não coincidem");
            return false;
        }

        if (!string.IsNullOrEmpty(requestDto.Senha))
        {
            usuario.Senha = _passwordHasher.HashPassword(usuario, requestDto.Senha);
        }

        usuario.TokenDeResetSenha = null;
        usuario.ExpiraResetToken = null;

        _usuarioRepository.Atualizar(usuario);
        await _usuarioRepository.UnitOfWork.Commit();
        
        await _emailService.SendEmailPasswordChangeConfirmation(usuario);

        return true;
    }

    
    private async Task<string> GerarToken(Usuario usuario)
    {
        var tokenHandle = new JwtSecurityTokenHandler();

        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, usuario.Email));

        var key = await _jwtService.GetCurrentSigningCredentials();
        var token = tokenHandle.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Emissor,
            Audience = _jwtSettings.ComumValidoEm,
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiracaoHoras),
            SigningCredentials = key
        });

        return tokenHandle.WriteToken(token);
    }
    
    private async Task<bool> Validar(Usuario usuario)
    {
        if (!usuario.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }

        var usuarioExistente = await _usuarioRepository.FirstOrDefault(c =>
            c.Id != usuario.Id && (c.Cpf == usuario.Cpf || c.Email == usuario.Email));
        
        if (usuarioExistente != null)
        {
            Notificator.Handle("Já existe um usuário cadastrado com essas identificações");
        }

        return !Notificator.HasNotification;
    }
    
    private string CreateRandomToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }
    
    private string CriarTokenEsqueceuSenha()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(8));
    }
}