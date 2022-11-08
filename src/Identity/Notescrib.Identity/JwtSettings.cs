namespace Notescrib.Identity;

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public TimeSpan TokenLifetime { get; set; } = TimeSpan.FromHours(1);
    public string Issuer { get; set; } = "Notescrib";
}
