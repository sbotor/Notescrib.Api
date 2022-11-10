namespace Notescrib.Identity;

public class JwtSettings
{
    public TimeSpan TokenLifetime { get; } = TimeSpan.FromHours(1);
    public string Key { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
}
