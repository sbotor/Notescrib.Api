using Notescrib.Notes.Core.Contracts;

namespace Notescrib.Notes.Application.Common.Providers;

public interface ISharingGuard
{
    IUserContextProvider UserContext { get; }
    
    bool CanEdit(IOwnable item);
    bool CanView(IShareable item);
    void GuardCanEdit(IOwnable item);
    void GuardCanView(IShareable item);
}
