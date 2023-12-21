using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Models;
using Notescrib.Models.Enums;

namespace Notescrib.Services;

internal class PermissionGuard : IPermissionGuard
{
    public PermissionGuard(IUserContextProvider userContext)
    {
        UserContext = userContext;
    }

    public IUserContextProvider UserContext { get; }

    public bool CanEdit(string ownerId) => UserContext.UserIdOrDefault == ownerId;

    public void GuardCanEdit(string ownerId)
    {
        if (!CanEdit(ownerId))
        {
            throw new ForbiddenException();
        }
    }

    public bool CanView(string ownerId, SharingInfo? sharingInfo = null)
        => (sharingInfo != null && sharingInfo.Visibility != VisibilityLevel.Private)
           || ownerId == UserContext.UserIdOrDefault;

    public void GuardCanView(string ownerId, SharingInfo? sharingInfo = null)
    {
        if (!CanView(ownerId, sharingInfo))
        {
            throw new ForbiddenException();
        }
    }
}
