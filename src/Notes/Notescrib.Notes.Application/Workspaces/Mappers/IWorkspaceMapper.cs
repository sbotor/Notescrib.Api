using Notescrib.Notes.Application.Workspaces.Commands;
using Notescrib.Notes.Application.Workspaces.Models;
using Notescrib.Notes.Core.Entities;
using Notescrib.Notes.Core.Models;

namespace Notescrib.Notes.Application.Workspaces.Mappers;

public interface IWorkspaceMapper
{
    WorkspaceOverview MapToOverview(Workspace item);
    WorkspaceDetails MapToDetails(Workspace item, IEnumerable<TreeNode<FolderOverview>> folders);
    Workspace MapToEntity(CreateWorkspace.Command item, string ownerId);
    Workspace UpdateEntity(UpdateWorkspace.Command item, Workspace original);
}
