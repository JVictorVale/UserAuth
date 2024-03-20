using Microsoft.AspNetCore.Mvc;
using UserAuth.Application.DTOs.Auth;
using UserAuth.Application.DTOs.Usuario;

namespace UserAuth.Application.Contracts;

public interface IAuthService
{
    Task<TokenDto?> Login(LoginUsuarioDto loginUsuarioDto);
    Task<UsuarioDto?> Cadastrar(RegistrarUsuarioDto registrarUsuarioDto);
    Task<bool> VerificarEmail(string token, string email);
    Task<bool> EsqueceuSenha(string email);
    Task<bool> ResetSenha(ResetSenhaDto requestDto);
}