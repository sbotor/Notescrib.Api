using System.Linq.Expressions;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Models;
using Notescrib.Notes.Models.Enums;

namespace Notescrib.Notes.Services;

internal class PermissionGuard : IPermissionGuard
{
    private readonly IUserContextProvider _userContext;

    public PermissionGuard(IUserContextProvider userContext)
    {
        _userContext = userContext;
    }

    public bool CanEdit(string ownerId) => _userContext.UserId == ownerId;

    public void GuardCanEdit(string ownerId)
    {
        if (!CanEdit(ownerId))
        {
            throw new ForbiddenException();
        }
    }

    public Expression<Func<T, bool>> ExpressionCanView<T>()
        where T : IShareable
        => x =>
            (_userContext.UserId == x.OwnerId || x.SharingInfo.Visibility == VisibilityLevel.Public)
            || (_userContext.UserId != null && x.SharingInfo.Visibility == VisibilityLevel.Hidden
                                            && x.SharingInfo.AllowedIds.Contains(_userContext.UserId));

    public bool CanView(string ownerId, SharingInfo? sharingInfo = null)
    {
        var userId = _userContext.UserId;
        if (userId == ownerId || sharingInfo?.Visibility == VisibilityLevel.Public)
        {
            return true;
        }

        return userId != null
               && sharingInfo is { Visibility: VisibilityLevel.Hidden }
               && sharingInfo.AllowedIds.Contains(userId);
    }

    public void GuardCanView(string ownerId, SharingInfo? sharingInfo = null)
    {
        if (!CanView(ownerId, sharingInfo))
        {
            throw new ForbiddenException();
        }
    }
}
