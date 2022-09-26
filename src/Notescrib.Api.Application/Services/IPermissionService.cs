using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Services;

public interface IPermissionService
{
    bool CanEdit(string ownerId);
    bool CanView(string ownerId, SharingDetails sharingDetails);
}
