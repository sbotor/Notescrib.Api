namespace Notescrib.Notes.Features.Workspaces.Models;

public class WorkspaceDetails : WorkspaceOverview
{
    public IReadOnlyCollection<FolderOverview> FolderTree { get; set; } = Array.Empty<FolderOverview>();
}
