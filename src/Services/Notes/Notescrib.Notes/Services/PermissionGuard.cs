using System.Linq.Expressions;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Models;
using Notescrib.Notes.Models.Enums;

namespace Notescrib.Notes.Services;

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
        => UserContext.IsAnonymous
            ? sharingInfo?.Visibility == VisibilityLevel.Public
            : sharingInfo?.Visibility != VisibilityLevel.Private
              || ownerId == UserContext.UserIdOrDefault;

    public void GuardCanView(string ownerId, SharingInfo? sharingInfo = null)
    {
        if (!CanView(ownerId, sharingInfo))
        {
            throw new ForbiddenException();
        }
    }
}
