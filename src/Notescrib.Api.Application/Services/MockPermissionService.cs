using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Services;

internal class MockPermissionService : IPermissionService
{
    public bool CanView(string ownerId, SharingDetails sharingDetails)
        => true;

    public bool CanEdit(string ownerId)
        => true;
}
