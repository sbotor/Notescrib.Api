namespace Notescrib.Identity.Features.Auth.Providers;

public interface IJwtProvider
{
    string GenerateToken(string userId);
}
