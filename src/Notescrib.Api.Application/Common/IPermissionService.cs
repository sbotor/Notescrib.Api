using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Common;

public interface IPermissionService
{
    IUserContextService User { get; }

    bool CanEdit(string ownerId);
    bool CanEdit(IOwnable item);
    bool CanView(string ownerId, SharingDetails sharingDetails);
    bool CanView(string ownerId, IShareable item);
    bool CanView(IOwnableShareable item);
}
