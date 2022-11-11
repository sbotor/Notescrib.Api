using Notescrib.Notes.Models;

namespace Notescrib.Notes.Services;

public interface IPermissionGuard
{
    bool CanEdit(string ownerId);
    void GuardCanEdit(string ownerId);
    bool CanView(string ownerId, SharingInfo? sharingInfo = null);
    void GuardCanView(string ownerId, SharingInfo? sharingInfo = null);
}
