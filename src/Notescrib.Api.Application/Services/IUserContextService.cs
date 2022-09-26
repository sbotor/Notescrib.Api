namespace Notescrib.Api.Application.Services;

internal interface IUserContextService
{
    string? UserId { get; }
    string? Username { get; }
    bool IsAuthenticated { get; }
}
