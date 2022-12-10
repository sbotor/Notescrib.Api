using Notescrib.Notes.Contracts;
using Notescrib.Notes.Utils.Tree;

namespace Notescrib.Notes.Features.Folders;

public class Folder : IChildrenCollectionTree<Folder>
{
    public const string RootId = "_root";
    
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public ICollection<Folder> Children { get; set; } = new List<Folder>();
    public bool IsRoot => Id == RootId;
    
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }

    public static Folder CreateRoot(IEnumerable<Folder>? children = null, DateTime? created = null)
        => new()
        {
            Id = RootId,
            Name = RootId,
            Children = children?.ToArray() ?? Array.Empty<Folder>(),
            Created = created ?? DateTime.MinValue
        };

    public static Folder CreateRoot(DateTime? created = null, params Folder[] children)
        => CreateRoot(children, created);
}
