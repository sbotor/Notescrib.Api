namespace Notescrib.Api.Application.Services;

internal interface IJwtProvider
{
    string GenerateToken(string userId, string userEmail);
}