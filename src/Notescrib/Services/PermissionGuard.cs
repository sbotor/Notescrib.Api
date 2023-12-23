using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Models;
using Notescrib.Models.Enums;

namespace Notescrib.Services;

public interface IPermissionGuard
{
    IUserContext UserContext { get; }
    
    ValueTask GuardCanEdit(string ownerId);
    ValueTask GuardCanView(string ownerId, SharingInfo? sharingInfo = null);
    
    ValueTask<bool> CanEdit(string ownerId);
    ValueTask<bool> CanView(string ownerId, SharingInfo? sharingInfo = null);
}

internal class PermissionGuard : IPermissionGuard
{
    public PermissionGuard(IUserContext userContext)
    {
        UserContext = userContext;
    }

    public IUserContext UserContext { get; }

    public async ValueTask<bool> CanEdit(string ownerId)
        => await GetUserId() == ownerId;

    public async ValueTask GuardCanEdit(string ownerId)
    {
        if (!await CanEdit(ownerId))
        {
            throw new ForbiddenException();
        }
    }

    public async ValueTask<bool> CanView(string ownerId, SharingInfo? sharingInfo = null)
        => (sharingInfo != null && sharingInfo.Visibility != VisibilityLevel.Private)
           || ownerId == await GetUserId();

    public async ValueTask GuardCanView(string ownerId, SharingInfo? sharingInfo = null)
    {
        if (!await CanView(ownerId, sharingInfo))
        {
            throw new ForbiddenException();
        }
    }

    private async ValueTask<string> GetUserId()
        => (await UserContext.GetUserInfo()).UserId;
}
