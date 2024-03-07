using FluentValidation;
using UserAuth.Domain.Entities;

namespace UserAuth.Domain.Validators;

public class UsuarioValidator : AbstractValidator<Usuario>
{
    public UsuarioValidator()
    {
        RuleFor(u => u.Nome)
            .NotEmpty()
            .WithMessage("O nome não pode ser vazio")
            .Length(3, 120)
            .WithMessage("O nome deve ter no mínimo 3 e no máximo 120 caracteres");
        
        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(80);
        
        RuleFor(u => u.Cpf)
            .NotEmpty()
            .WithMessage("Cpf não pode ser vazio")
            .Length(11, 14)
            .WithMessage("Cpf deve ter no mínimo 11 e no máximo 14 caracteres");
        
        RuleFor(a => a.Senha)
            .NotEmpty()
            .WithMessage("A senha não pode ser vazia")
            .MinimumLength(6)
            .WithMessage("A senha deve ter no mínimo 6 caracteres");

    }
}