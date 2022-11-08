namespace Notescrib.Identity.Auth.Providers;

internal interface IJwtProvider
{
    string GenerateToken(string userId, string userEmail);
}