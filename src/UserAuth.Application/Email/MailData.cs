namespace UserAuth.Application.Email;

public class MailData
{
    public string EmailSubject { get; set; } = null!;
    public string EmailBody { get; set; } = null!;
    public string EmailToId { get; set; } = null!;
}