using AutoMapper;
using Microsoft.AspNetCore.Identity;
using UserAuth.Application.Contracts;
using UserAuth.Application.DTOs.Usuario;
using UserAuth.Application.Notifications;
using UserAuth.Domain.Contracts.Repositories;
using UserAuth.Domain.Entities;

namespace UserAuth.Application.Services;

public class UsuarioService : BaseService, IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    
    public UsuarioService(IMapper mapper, INotificator notificator, IUsuarioRepository usuarioRepository, IPasswordHasher<Usuario> passwordHasher) : base(mapper, notificator)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UsuarioDto?> Adicionar(AdicionarUsuarioDto usuarioDto)
    {
        var usuario = Mapper.Map<Usuario>(usuarioDto);
        
        if (usuarioDto.Senha != usuarioDto.ConfirmarSenha)
        {
            Notificator.Handle("As senhas informadas não coincidem");
            return null;
        }
        
        if (!await Validar(usuario))
        {
            return null;
        }
        
        usuario.Senha = _passwordHasher.HashPassword(usuario, usuario.Senha);
        _usuarioRepository.Cadastrar(usuario);
        if (await _usuarioRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<UsuarioDto>(usuario);
        }

        Notificator.Handle("Não foi possível cadastrar o usuário");
        return null;
    }

    public async Task<UsuarioDto?> Atualizar(int id, AtualizarUsuarioDto usuarioDto)
    {
        if (id != usuarioDto.Id)
        {
            Notificator.Handle("Os ids não conferem!");
            return null;
        }

        var usuario = await _usuarioRepository.ObterPorId(id);
        if (usuario == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        if (!string.IsNullOrEmpty(usuarioDto.Senha) && usuarioDto.Senha != usuarioDto.ConfirmarSenha)
        {
            Notificator.Handle("As senhas informadas não coincidem");
            return null;
        }
        
        Mapper.Map(usuarioDto, usuario);
        if (!await Validar(usuario))
        {
            return null;
        }
        
        if (!string.IsNullOrEmpty(usuarioDto.Senha))
        {
            usuario.Senha = _passwordHasher.HashPassword(usuario, usuarioDto.Senha);
        }
        
        _usuarioRepository.Atualizar(usuario);
        
        if (await _usuarioRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<UsuarioDto>(usuario);
        }
        
        Notificator.Handle("Não foi possível alterar o usuário");
        return null;
    }

    public async Task<UsuarioDto?> ObterPorId(int id)
    {
        var usuario = await _usuarioRepository.ObterPorId(id);

        if (usuario != null)
            return Mapper.Map<UsuarioDto>(usuario);
        
        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task<List<UsuarioDto>?> ObterTodos()
    {
        var usuarios = await _usuarioRepository.ObterTodos();
        if (usuarios != null)
            return Mapper.Map<List<UsuarioDto>>(usuarios);
        
        Notificator.Handle("Não existe usuário cadastrado");
        return null;
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
}