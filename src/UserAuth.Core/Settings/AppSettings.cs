namespace UserAuth.Core.Settings;

public class AppSettings
{
    public int ExpirationHours { get; set; }
    public string UrlApi { get; set; } = string.Empty;
}