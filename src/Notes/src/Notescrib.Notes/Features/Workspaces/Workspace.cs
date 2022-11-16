using Notescrib.Notes.Contracts;

namespace Notescrib.Notes.Features.Workspaces;

public class Workspace
{
    public string Id { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public IReadOnlyCollection<Folder> Folders { get; set; } = Array.Empty<Folder>();
}

public class Folder : IChildrenCollectionTree<Folder>
{
    public string Name { get; set; } = null!;
    public ICollection<Folder> Children { get; set; } = new List<Folder>();
}
