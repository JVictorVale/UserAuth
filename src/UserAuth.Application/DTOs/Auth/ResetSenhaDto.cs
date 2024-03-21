namespace UserAuth.Application.DTOs.Auth;

public class ResetSenhaDto
{
    public string Token { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public string ConfirmarSenha { get; set; } = null!;
}