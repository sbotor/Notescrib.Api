using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Notescrib.Api.Application.Common.Services;

internal class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => GetClaim(ClaimTypes.NameIdentifier);
    public string? Email => GetClaim(ClaimTypes.Email);
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    private string? GetClaim(string claimType)
        => _httpContextAccessor.HttpContext?.User?.Claims
            ?.FirstOrDefault(c => c.Type == claimType)?.Value;
}
