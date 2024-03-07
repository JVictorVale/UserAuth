using UserAuth.Application.DTOs.Auth;

namespace UserAuth.Application.Contracts;

public interface IAuthService
{
    Task<TokenDto?> Login(UsuarioLoginDto usuarioLoginDto);
}