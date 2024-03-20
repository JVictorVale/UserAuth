using UserAuth.Domain.Entities;

namespace UserAuth.Application.Email;

public interface IEmailService
{
    Task SendEmailVerification(Usuario usuario);
    Task SendEmailRecoverPassword(Usuario usuario);
    Task SendEmailPasswordChangeConfirmation(Usuario usuario);

}