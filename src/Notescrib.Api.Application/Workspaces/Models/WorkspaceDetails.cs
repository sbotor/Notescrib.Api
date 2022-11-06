using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Models;

public class WorkspaceDetails : WorkspaceOverview
{
    public IReadOnlyCollection<TreeNode<FolderOverview>> Folders { get; set; } = Array.Empty<TreeNode<FolderOverview>>();
}
