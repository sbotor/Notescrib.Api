using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Common;

public interface ISharingService
{
    IUserContextProvider UserContext { get; }

    bool CanEdit(string ownerId);
    bool CanEdit(IOwnable item);
    bool CanView(string ownerId, SharingInfo sharingInfo);
    bool CanView(IShareable item);
}
