namespace Notescrib.Api.Application.Common;

public interface IUserContextService
{
    string? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}
