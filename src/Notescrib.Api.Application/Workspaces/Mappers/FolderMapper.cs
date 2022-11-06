using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Mappers;

public class FolderMapper : IFolderMapper
{
    public FolderOverview MapToOverview(Folder item)
        => new() { Id = item.Id, Name = item.Name, SharingInfo = item.SharingInfo, Updated = item.Updated };

    public Folder MapToEntity(CreateFolder.Command item, string ownerId, SharingInfo sharingInfo)
        => new()
        {
            Name = item.Name,
            ParentId = item.ParentId,
            WorkspaceId = item.WorkspaceId,
            OwnerId = ownerId,
            SharingInfo = sharingInfo
        };

    public Folder UpdateEntity(UpdateFolder.Command item, Folder original)
    {
        original.ParentId = item.ParentId;
        original.Name = item.Name;
        original.SharingInfo = item.SharingInfo;

        return original;
    }
}
