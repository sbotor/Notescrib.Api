namespace Notescrib.Api.Application.Services.Auth;

internal interface IJwtProvider
{
    string GenerateToken(string userId, string userEmail);
}