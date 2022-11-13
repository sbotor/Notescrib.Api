namespace Notescrib.Notes.Services;

public interface IUserContextProvider
{
    string? UserId { get; }
    string? Email { get; }
}
