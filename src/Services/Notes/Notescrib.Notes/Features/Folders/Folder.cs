using Notescrib.Notes.Contracts;

namespace Notescrib.Notes.Features.Folders;

public class Folder : IChildrenTree<ICollection<Folder>, Folder>
{
    public const string RootId = "_root";
    
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public ICollection<Folder> ChildrenIds { get; set; } = new List<Folder>();
    public bool IsRoot => Id == RootId;
    
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }

    public static Folder CreateRoot(IEnumerable<Folder>? children = null, DateTime? created = null)
        => new()
        {
            Id = RootId,
            Name = RootId,
            ChildrenIds = children?.ToArray() ?? Array.Empty<Folder>(),
            Created = created ?? DateTime.MinValue
        };

    public static Folder CreateRoot(DateTime? created = null, params Folder[] children)
        => CreateRoot(children, created);
}
