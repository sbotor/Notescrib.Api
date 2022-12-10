using Notescrib.Notes.Contracts;

namespace Notescrib.Notes.Features.Notes;

public class NoteSection : IChildrenCollectionTree<NoteSection>
{
    public const string RootId = "_root";
    
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Content { get; set; } = null!;
    public ICollection<NoteSection> Children { get; set; } = new List<NoteSection>();

    public static NoteSection CreateRoot(IEnumerable<NoteSection> children)
        => new() { Id = RootId, Name = RootId, Children = children.ToArray() };
    public static NoteSection CreateRoot(params NoteSection[] children)
        => CreateRoot(children.AsEnumerable());
}
