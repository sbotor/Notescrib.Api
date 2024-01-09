using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Notescrib.Core.Services;

public record UserInfo(string UserId)
{
    public bool IsAnonymous => UserId == string.Empty;

    public static UserInfo Empty()
        => new(string.Empty);
}

public interface IUserContext
{
    ValueTask<UserInfo> GetUserInfo(CancellationToken cancellationToken = default);
    ValueTask<string> GetUserId(CancellationToken cancellationToken = default);
}

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private UserInfo? _userInfo;
    
    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async ValueTask<UserInfo> GetUserInfo(CancellationToken cancellationToken = default)
    {
        if (_userInfo is not null)
        {
            return _userInfo;
        }

        _userInfo = ExtractInfoCore();
        await AfterUserExtraction(_userInfo, cancellationToken);

        return _userInfo;
    }

    public async ValueTask<string> GetUserId(CancellationToken cancellationToken = default)
    {
        var user = await GetUserInfo(cancellationToken);
        return user.UserId;
    }

    protected virtual ValueTask AfterUserExtraction(
        UserInfo user,
        CancellationToken cancellationToken)
        => ValueTask.CompletedTask;

    private UserInfo ExtractInfoCore()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return UserInfo.Empty();
        }

        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? string.Empty;

        return new(userId);
    }

    public string? UserIdOrDefault => GetClaim(ClaimTypes.NameIdentifier);

    private string? GetClaim(string claimType)
        => _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;

    private bool CheckIsAuthenticated()
        => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true
           && !string.IsNullOrEmpty(UserIdOrDefault);
}
