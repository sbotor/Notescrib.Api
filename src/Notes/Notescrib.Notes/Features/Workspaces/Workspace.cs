namespace Notescrib.Notes.Features.Workspaces;

public class Workspace
{
    public string Id { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public FolderTree FolderTree { get; set; } = new();
}

public class Folder
{
    public string Name { get; set; } = null!;
    public ICollection<Folder> Children { get; set; } = new List<Folder>();
}
