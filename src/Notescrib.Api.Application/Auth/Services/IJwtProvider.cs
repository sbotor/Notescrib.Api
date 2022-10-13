namespace Notescrib.Api.Application.Auth.Services;

internal interface IJwtProvider
{
    string GenerateToken(string userId, string userEmail);
}