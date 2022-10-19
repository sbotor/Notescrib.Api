using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Enums;

namespace Notescrib.Api.Application.Common;

internal class PermissionService : IPermissionService
{
    private readonly IUserContextService _userContextService;

    public PermissionService(IUserContextService userContextService)
    {
        _userContextService = userContextService;
    }

    public bool CanEdit(string ownerId) => _userContextService.UserId == ownerId;
    public bool CanEdit(IOwnable item) => CanEdit(item.OwnerId);

    public bool CanView(string ownerId, SharingDetails sharingDetails)
    {
        var userId = _userContextService.UserId;
        if (sharingDetails.Visibility == VisibilityLevel.Public
            || userId == ownerId)
        {
            return true;
        }

        if (sharingDetails.Visibility == VisibilityLevel.Hidden)
        {
            return userId != null
                    && sharingDetails.AllowedUserIds.Contains(userId);
        }

        return false;
    }

    public bool CanView(string ownerId, IShareable item) => CanView(ownerId, item.SharingDetails);
    public bool CanView(IOwnableShareable item) => CanView(item.OwnerId, item.SharingDetails);
}
