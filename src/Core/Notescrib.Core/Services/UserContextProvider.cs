using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Notescrib.Core.Services;

public class UserContextProvider : IUserContextProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId => UserIdOrDefault
                            ?? throw new InvalidOperationException("No user ID found.");

    public string? UserIdOrDefault => GetClaim(ClaimTypes.NameIdentifier);

    public bool IsAnonymous => CheckIsAuthenticated()
                               ?? throw new InvalidOperationException("No user identity found.");

    private string? GetClaim(string claimType)
        => _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;

    private bool? CheckIsAuthenticated()
    {
        var isAuthenticated = _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated;
        return !isAuthenticated ?? null;
    }
}
