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

    public string UserId => GetClaim(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("No user ID found.");
    public string Email => GetClaim(ClaimTypes.Email) ?? throw new InvalidOperationException("No user email found.");

    private string? GetClaim(string claimType)
        => _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
}
