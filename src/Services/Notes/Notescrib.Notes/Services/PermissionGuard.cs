using System.Linq.Expressions;
using Notescrib.Core.Models.Exceptions;
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

    public bool CanEdit(string ownerId) => UserContext.UserId == ownerId;

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
            (UserContext.UserId == x.OwnerId || x.SharingInfo.Visibility == VisibilityLevel.Public)
            || (x.SharingInfo.Visibility == VisibilityLevel.Hidden
                && x.SharingInfo.AllowedIds.Contains(UserContext.UserId));

    public bool CanView(string ownerId, SharingInfo? sharingInfo = null)
    {
        var userId = UserContext.UserId;
        if (userId == ownerId || sharingInfo?.Visibility == VisibilityLevel.Public)
        {
            return true;
        }

        return sharingInfo is { Visibility: VisibilityLevel.Hidden }
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
