using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Common;

public interface ISharingService
{
    IUserContextProvider UserContext { get; }
    
    bool CanEdit(IOwnable item);
    bool CanView(IShareable item);
    void GuardCanEdit(IOwnable item);
    void GuardCanView(IShareable item);
}
