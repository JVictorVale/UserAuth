namespace UserAuth.Application.DTOs.Auth;

public class UsuarioLoginDto
{
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;
}