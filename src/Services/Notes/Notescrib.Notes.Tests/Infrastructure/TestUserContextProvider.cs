using Notescrib.Notes.Services;

namespace Notescrib.Notes.Tests.Infrastructure;

public class TestUserContextProvider : IUserContextProvider
{
    public string? UserId { get; set; } = "1";
    public string? Email { get; set; }
}
