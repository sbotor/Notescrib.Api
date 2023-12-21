namespace Notescrib.Features.Folders.Models;

public class FolderOverview
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}
