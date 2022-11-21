using Notescrib.Notes.Services;

namespace Notescrib.Notes.Tests.Infrastructure;

public class TestUserContextProvider : IUserContextProvider
{
    public string? UserId { get; set; }
    public string? Email { get; set; }
}
