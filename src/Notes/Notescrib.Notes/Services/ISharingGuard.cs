using Notescrib.Notes.Application.Models;

namespace Notescrib.Notes.Application.Services;

public interface ISharingGuard
{
    bool CanEdit(string ownerId);
    void GuardCanEdit(string ownerId);
    bool CanView(string ownerId, SharingInfo sharingInfo);
    void GuardCanView(string ownerId, SharingInfo sharingInfo);
}
