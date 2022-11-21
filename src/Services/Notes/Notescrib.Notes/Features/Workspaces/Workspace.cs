using System.Collections;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Utils.Tree;

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
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public ICollection<Folder> Children { get; set; } = new List<Folder>();
    
    public IEnumerable<Folder> EnumerateChildren() => new BfsTreeEnumerable<Folder>(Children);
}
