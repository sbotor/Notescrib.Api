namespace Notescrib.Api.Application.Common;

public interface IUserContextProvider
{
    string? UserId { get; }
    string? Email { get; }
}
