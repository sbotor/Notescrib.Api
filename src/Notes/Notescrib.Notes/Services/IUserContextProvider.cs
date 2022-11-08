namespace Notescrib.Notes.Application.Services;

public interface IUserContextProvider
{
    string? UserId { get; }
    string? Email { get; }
}
