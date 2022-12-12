using System.Linq.Expressions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Models;

namespace Notescrib.Notes.Services;

public interface IPermissionGuard
{
    void GuardCanEdit(string ownerId);
    void GuardCanView(string ownerId, SharingInfo? sharingInfo = null);
    
    public IUserContextProvider UserContext { get; }

    Expression<Func<T, bool>> ExpressionCanView<T>()
        where T : IShareable;

    bool CanView(string ownerId, SharingInfo? sharingInfo = null);
}
