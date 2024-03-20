namespace UserAuth.Application.DTOs.Auth;

public class ResetSenhaDto
{
    public string Token { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}