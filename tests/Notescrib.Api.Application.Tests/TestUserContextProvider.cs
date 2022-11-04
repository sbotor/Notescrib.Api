using Castle.Core.Resource;
using Notescrib.Api.Application.Common;

namespace Notescrib.Api.Application.Tests;

internal class TestUserContextProvider : IUserContextProvider
{
    public static TestUserContextProvider First => new(Id.User.First);

    public TestUserContextProvider(string? userId = null, string? email = null)
    {
        UserId = userId;
        Email = email;
    }

    public string? UserId { get; set; }

    public string? Email { get; set; }
}
