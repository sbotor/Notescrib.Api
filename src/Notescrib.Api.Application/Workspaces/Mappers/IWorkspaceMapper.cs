using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Mappers;

public interface IWorkspaceMapper
{
    WorkspaceOverview MapToOverview(Workspace item);
    WorkspaceDetails MapToDetails(Workspace item, IEnumerable<TreeNode<FolderOverview>> folders);
    Workspace MapToEntity(CreateWorkspace.Command item, string ownerId);
    Workspace UpdateEntity(UpdateWorkspace.Command item, Workspace original);
}
