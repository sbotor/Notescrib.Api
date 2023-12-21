namespace Notescrib.Core.Services;

public interface IUserContextProvider
{
    string UserId { get; }
    bool IsAnonymous { get; }
    string? UserIdOrDefault { get; }
}
