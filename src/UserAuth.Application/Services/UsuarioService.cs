using AutoMapper;
using Microsoft.AspNetCore.Http;
using UserAuth.Application.Contracts;
using UserAuth.Application.DTOs.Usuario;
using UserAuth.Application.Notifications;
using UserAuth.Core.Enums;
using UserAuth.Domain.Contracts.Repositories;
using UserAuth.Domain.Entities;

namespace UserAuth.Application.Services;

public class UsuarioService : BaseService, IUsuarioService
{
    private readonly IFileService _fileService;
    private readonly IUsuarioRepository _usuarioRepository;
    
    public UsuarioService(IMapper mapper, INotificator notificator, IUsuarioRepository usuarioRepository, IFileService fileService) : base(mapper, notificator)
    {
        _usuarioRepository = usuarioRepository;
        _fileService = fileService;
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
        
        Mapper.Map(usuarioDto, usuario);
        if (!await Validar(usuario))
        {
            return null;
        }
        
        if (usuarioDto.Fotos is { Length: > 0 } && !await ManterFoto(usuarioDto.Fotos, usuario))
        {
            usuario.Foto = await _fileService.Upload(usuarioDto.Fotos, EUploadPath.FotoUsuarios);
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
    
    private async Task<bool> ManterFoto(IFormFile foto, Usuario usuario)
    {
        if (!string.IsNullOrWhiteSpace(usuario.Foto) && !_fileService.Apagar(new Uri(usuario.Foto)))
        {
            Notificator.Handle("Não foi possível remover a foto anterior.");
            return false;
        }

        usuario.Foto = await _fileService.Upload(foto, EUploadPath.FotoUsuarios);
        return true;
    }
}