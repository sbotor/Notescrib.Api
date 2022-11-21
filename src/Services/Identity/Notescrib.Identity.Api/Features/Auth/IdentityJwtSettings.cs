using Notescrib.Core.Api.Configuration;

namespace Notescrib.Identity.Api.Features.Auth;

public class IdentityJwtSettings : JwtSettings
{
    public TimeSpan TokenLifetime { get; } = TimeSpan.FromHours(1);
}
