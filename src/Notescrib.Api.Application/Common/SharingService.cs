using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Enums;
using Notescrib.Api.Core.Exceptions;

namespace Notescrib.Api.Application.Common;

internal class SharingService : ISharingService
{
    public IUserContextProvider UserContext { get; }

    public SharingService(IUserContextProvider userContext)
    {
        UserContext = userContext;
    }

    private bool CanEdit(string ownerId) => UserContext.UserId == ownerId;
    
    public bool CanEdit(IOwnable item) => CanEdit(item.OwnerId);

    public void GuardCanEdit(IOwnable item)
    {
        if (!CanEdit(item.OwnerId))
        {
            throw new ForbiddenException();
        }
    }

    private bool CanView(string ownerId, SharingInfo sharingInfo)
    {
        var userId = UserContext.UserId;
        if (sharingInfo.Visibility == VisibilityLevel.Public
            || userId == ownerId)
        {
            return true;
        }

        if (sharingInfo.Visibility == VisibilityLevel.Hidden)
        {
            return userId != null
                && sharingInfo.AllowedUserIds.Contains(userId);
        }

        return false;
    }

    public bool CanView(IShareable item) => CanView(item.OwnerId, item.SharingInfo);

    public void GuardCanView(IShareable item)
    {
        if (!CanView(item))
        {
            throw new ForbiddenException();
        }
    }
}
