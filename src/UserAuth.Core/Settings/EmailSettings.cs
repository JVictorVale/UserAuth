namespace UserAuth.Core.Settings;

public class EmailSettings
{
    public string Name { get; set; } = string.Empty; 
    public string User { get; set; } = string.Empty; 
    public string Password { get; set; } = string.Empty;
    public string Server { get; set; } = string.Empty;
    public int Port { get; set; }
}