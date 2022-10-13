using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Enums;

namespace Notescrib.Api.Application.Common.Services;

internal class PermissionService : IPermissionService
{
    private readonly IUserContextService _userContextService;

    public PermissionService(IUserContextService userContextService)
    {
        _userContextService = userContextService;
    }

    public bool CanEdit(string ownerId) => _userContextService.UserId == ownerId;

    public bool CanView(string ownerId, SharingDetails sharingDetails)
        => _userContextService.UserId == ownerId
            || sharingDetails.Visibility == Visibility.Public
            || sharingDetails.Visibility == Visibility.Hidden
                && sharingDetails.AllowedUserIds.Contains(_userContextService.UserId ?? string.Empty);
}
