namespace Notescrib.Api.Application.Workspaces.Models;

public class WorkspaceDetails : WorkspaceOverview
{
    public IReadOnlyCollection<FolderOverviewTree.Node> Folders { get; set; } = Array.Empty<FolderOverviewTree.Node>();
}
