using Notescrib.Core.Api.Configuration;

namespace Notescrib.Identity.Api;

public class IdentityJwtSettings : JwtSettings
{
    public TimeSpan TokenLifetime { get; set; } = TimeSpan.FromHours(1);
}
