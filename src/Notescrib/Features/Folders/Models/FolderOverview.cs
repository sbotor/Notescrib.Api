namespace Notescrib.Features.Folders.Models;

public class FolderOverview
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}
