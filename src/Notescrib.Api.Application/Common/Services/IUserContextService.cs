namespace Notescrib.Api.Application.Common.Services;

public interface IUserContextService
{
    string? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}
