namespace Notescrib.Api.Application.Services;

public interface IUserContextService
{
    string? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}
