using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using UserAuth.Application.Notifications;
using UserAuth.Core.Settings;
using UserAuth.Domain.Entities;

namespace UserAuth.Application.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly AppSettings _appSettings;

    public EmailService(IOptions<EmailSettings> emailSettings, IOptions<AppSettings> appSettings)
    {
        _emailSettings = emailSettings.Value;
        _appSettings = appSettings.Value;
    }
    
    public async Task SendEmailVerification(Usuario usuario)
    {
        var url = $"{_appSettings.UrlApi}auth/usuariosauth/verificar?token={usuario.TokenDeVerificacao}&email={usuario.Email}";
        var body = 
            $"Olá {usuario.Nome}, bem-vindo ao UserAuth! Por favor, confirme seu registro clicando no link abaixo:<br>" +
            $"<a href='{url}'>Clique aqui para confirmar seu e-mail</a><br>" +
            $"Também pode copiar e colar este URL em seu navegador: {url}<br>" +
            $"Este link de confirmação é válido por {_appSettings.ExpirationHours} horas.";


        var mailData = new MailData
        {
            EmailSubject = "Verificação de E-mail",
            EmailBody = body,
            EmailToId = usuario.Email
        };

        await SendEmailAsync(mailData);
    }

    public async Task SendEmailRecoverPassword(Usuario usuario)
    {
        var body = $"Olá {usuario.Nome},<br>" +
                   $"Segue o seu token de recuperação de senha:<br>" +
                   $"{usuario.TokenDeResetSenha}<br>" +
                   $"Lembrando que o token é válido por {_appSettings.ExpirationHours} horas desde o pedido de troca da senha.";
        
        var mailData = new MailData
        {
            EmailSubject = "Recuperação de Senha",
            EmailBody = body,
            EmailToId = usuario.Email
        };

        await SendEmailAsync(mailData);
    }

    public async Task SendEmailPasswordChangeConfirmation(Usuario usuario)
    {
        var body = $"Olá {usuario.Nome},<br>" +
                   $"Sua senha foi alterada com sucesso.<br>" +
                   $"Agora você já pode efetuar o login com sua nova senha.<br>";

        
        var mailData = new MailData
        {
            EmailSubject = "Confirmação de Alteração de Senha",
            EmailBody = body,
            EmailToId = usuario.Email
        };

        await SendEmailAsync(mailData);
    }

    public async Task SendEmailAsync(MailData mailData)
    {
        var toEmail = mailData.EmailToId;
        var user = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(_emailSettings.User));
        var password = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(_emailSettings.Password));

        var smtpClient = new SmtpClient(_emailSettings.Server)
        {
            Port = _emailSettings.Port,
            Credentials = new NetworkCredential(user, password),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage(user, toEmail)
        {
            Subject = mailData.EmailSubject,
            Body = mailData.EmailBody,
            IsBodyHtml = true
        };

        try
        {
            await Task.Run(() => smtpClient.Send(mailMessage));
        }
        catch (Exception)
        {
            var notificator = new Notificator();
            notificator.Handle("An error occurred while sending the email");
        }
    }
}