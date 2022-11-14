namespace Notescrib.Notes.Features.Workspaces.Models;

public class FolderOverview
{
    public string Name { get; set; } = null!;
    public ICollection<FolderOverview> Children { get; set; } = Array.Empty<FolderOverview>();
}
