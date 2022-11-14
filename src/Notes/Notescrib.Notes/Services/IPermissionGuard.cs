using System.Linq.Expressions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Models;

namespace Notescrib.Notes.Services;

public interface IPermissionGuard
{
    bool CanEdit(string ownerId);
    void GuardCanEdit(string ownerId);
    bool CanView(string ownerId, SharingInfo? sharingInfo = null);
    void GuardCanView(string ownerId, SharingInfo? sharingInfo = null);

    Expression<Func<T, bool>> ExpressionCanView<T>()
        where T : IShareable;
}
