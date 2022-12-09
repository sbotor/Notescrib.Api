using Notescrib.Notes.Contracts;
using Notescrib.Notes.Utils.Tree;

namespace Notescrib.Notes.Features.Workspaces;

public class Folder : IChildrenCollectionTree<Folder>
{
    public const string RootName = "__root__";
    public const string RootId = "_";
    
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public ICollection<Folder> Children { get; set; } = new List<Folder>();
    public bool IsRoot => Id == RootId;
    
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    
    public IEnumerable<Folder> EnumerateChildren() => new BfsTreeEnumerable<Folder>(CreateRoot(Children)).Skip(1);

    public static Folder CreateRoot(IEnumerable<Folder>? children = null, DateTime? created = null)
        => new()
        {
            Id = RootId,
            Name = RootName,
            Children = children?.ToArray() ?? Array.Empty<Folder>(),
            Created = created ?? DateTime.MinValue
        };

    public static Folder CreateRoot(DateTime? created = null, params Folder[] children)
        => CreateRoot(children, created);
}
