using FluentValidation.Results;
using UserAuth.Domain.Contracts;
using UserAuth.Domain.Validators;

namespace UserAuth.Domain.Entities;

public class Usuario : Entity, IAggregateRoot
{
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Cpf { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public string TokenDeVerificacao { get; set; } = null!;
    public bool ContaVerificada { get; set; }
    public DateTime VerificadoEm { get; set; }
    public string? TokenDeResetSenha { get; set; }
    public DateTime? ExpiraResetToken { get; set; }

    public override bool Validar(out ValidationResult validationResult)
    {
        validationResult = new UsuarioValidator().Validate(this);
        return validationResult.IsValid;
    }
}