using Notescrib.Notes.Models;

namespace Notescrib.Notes.Services;

public interface IPermissionGuard
{
    void GuardCanEdit(string ownerId);
    void GuardCanView(string ownerId, SharingInfo? sharingInfo = null);
    
    public IUserContextProvider UserContext { get; }
    bool CanEdit(string ownerId);
}
