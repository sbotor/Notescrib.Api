namespace Notescrib.Identity.Features.Auth.Providers;

internal interface IJwtProvider
{
    string GenerateToken(string userId, string userEmail);
}