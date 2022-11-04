using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Notescrib.Api.Application.Common;

internal class UserContextProvider : IUserContextProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => GetClaim(ClaimTypes.NameIdentifier);
    public string? Email => GetClaim(ClaimTypes.Email);

    private string? GetClaim(string claimType)
        => _httpContextAccessor.HttpContext?.User?.Claims
            ?.FirstOrDefault(c => c.Type == claimType)?.Value;
}
