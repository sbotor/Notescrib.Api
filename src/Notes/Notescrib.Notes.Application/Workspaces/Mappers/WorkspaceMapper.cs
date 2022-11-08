using Notescrib.Notes.Application.Workspaces.Commands;
using Notescrib.Notes.Application.Workspaces.Models;
using Notescrib.Notes.Core.Entities;
using Notescrib.Notes.Core.Models;

namespace Notescrib.Notes.Application.Workspaces.Mappers;

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
