using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserAuth.Domain.Entities;

namespace UserAuth.Infra.Mappings;

public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder
            .Property(u => u.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder
            .Property(u => u.Email)
            .HasMaxLength(120)
            .IsRequired();
        
        builder.Property(c => c.Cpf)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(a => a.Senha)
            .HasMaxLength(255)
            .IsRequired();
        
        builder
            .Property(u => u.TokenDeVerificacao)
            .HasMaxLength(255)
            .IsRequired();
        
        builder
            .Property(u => u.ContaVerificada)
            .HasDefaultValue(false);

        builder
            .Property(u => u.TokenDeResetSenha)
            .HasMaxLength(255)
            .IsRequired(false);

        builder
            .Property(u => u.ExpiraResetToken)
            .IsRequired(false);
    }
}