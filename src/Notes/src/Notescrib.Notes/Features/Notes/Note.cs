using Notescrib.Notes.Contracts;
using Notescrib.Notes.Models;

namespace Notescrib.Notes.Features.Notes;

public class Note : IShareable
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string WorkspaceId { get; set; } = null!;
    public string FolderId { get; set; } = null!;
    public IReadOnlyCollection<NoteSection> Contents { get; set; } = Array.Empty<NoteSection>();
    public ICollection<string> Labels { get; set; } = new List<string>();
    public string OwnerId { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = null!;
}

public class NoteSection : IChildrenCollectionTree<NoteSection>
{
    public string Name { get; set; } = null!;
    public string Content { get; set; } = null!;
    public ICollection<NoteSection> Children { get; set; } = new List<NoteSection>();
}
