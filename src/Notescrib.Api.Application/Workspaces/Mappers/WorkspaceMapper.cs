using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Mappers;

public class WorkspaceMapper : IWorkspaceMapper
{
    public WorkspaceOverview MapToOverview(Workspace item)
        => new()
        {
            Id = item.Id,
            OwnerId = item.OwnerId,
            Name = item.Name,
            SharingInfo = item.SharingInfo,
            Updated = item.Updated
        };

    public WorkspaceDetails MapToDetails(Workspace item, IEnumerable<TreeNode<FolderOverview>> folders)
        => new()
        {
            Id = item.Id,
            OwnerId = item.OwnerId,
            Name = item.Name,
            SharingInfo = item.SharingInfo,
            Updated = item.Updated,
            Folders = folders.ToArray()
        };

    public Workspace MapToEntity(CreateWorkspace.Command item, string ownerId)
        => new() { Name = item.Name, SharingInfo = item.SharingInfo, OwnerId = ownerId };

    public Workspace UpdateEntity(UpdateWorkspace.Command item, Workspace original)
    {
        original.Name = item.Name;
        original.SharingInfo = item.SharingInfo;

        return original;
    }
}
