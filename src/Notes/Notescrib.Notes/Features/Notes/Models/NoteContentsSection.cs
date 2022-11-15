using Notescrib.Notes.Contracts;

namespace Notescrib.Notes.Features.Notes.Models;

public class NoteContentsSection : IChildrenCollectionTree<NoteContentsSection>
{
    public string Name { get; set; } = null!;
    public string Content { get; set; } = null!;
    public ICollection<NoteContentsSection> Children { get; set; } = Array.Empty<NoteContentsSection>();
}
