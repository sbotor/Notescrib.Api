namespace Notescrib.Notes.Application.Common.Providers;

public interface IUserContextProvider
{
    string? UserId { get; }
    string? Email { get; }
}
