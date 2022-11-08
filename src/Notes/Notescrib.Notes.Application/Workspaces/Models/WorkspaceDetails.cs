using Notescrib.Notes.Core.Models;

namespace Notescrib.Notes.Application.Workspaces.Models;

public class WorkspaceDetails : WorkspaceOverview
{
    public IReadOnlyCollection<TreeNode<FolderOverview>> Folders { get; set; } = Array.Empty<TreeNode<FolderOverview>>();
}
