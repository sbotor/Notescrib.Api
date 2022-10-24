namespace Notescrib.Api.Application.Workspaces.Models;

public class WorkspaceDetails : WorkspaceOverview
{
    public IReadOnlyCollection<FolderOverview> Folders { get; set; } = Array.Empty<FolderOverview>();
}
