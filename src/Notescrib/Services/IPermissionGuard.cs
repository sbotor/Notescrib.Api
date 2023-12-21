using Notescrib.Core.Services;
using Notescrib.Models;

namespace Notescrib.Services;

public interface IPermissionGuard
{
    void GuardCanEdit(string ownerId);
    void GuardCanView(string ownerId, SharingInfo? sharingInfo = null);
    
    public IUserContextProvider UserContext { get; }
    bool CanEdit(string ownerId);
    bool CanView(string ownerId, SharingInfo? sharingInfo = null);
}
