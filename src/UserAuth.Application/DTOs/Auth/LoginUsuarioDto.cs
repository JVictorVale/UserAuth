namespace UserAuth.Application.DTOs.Auth;

public class LoginUsuarioDto
{
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;
}