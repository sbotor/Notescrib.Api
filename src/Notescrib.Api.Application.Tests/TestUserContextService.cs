using Notescrib.Api.Application.Services;

namespace Notescrib.Api.Application.Tests;

internal class TestUserContextService : IUserContextService
{
    public string? UserId { get; set; }

    public string? Email { get; set; }

    public bool IsAuthenticated { get; set; }
}
