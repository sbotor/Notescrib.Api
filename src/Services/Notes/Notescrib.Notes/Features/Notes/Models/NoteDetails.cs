using Notescrib.Notes.Models;

namespace Notescrib.Notes.Features.Notes.Models;

public class NoteDetails
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string WorkspaceId { get; set; } = null!;
    public string FolderId { get; set; } = null!;
    public IReadOnlyCollection<NoteContentsSection> Contents { get; set; } = Array.Empty<NoteContentsSection>();
    public ICollection<string> Tags { get; set; } = new List<string>();
    public string OwnerId { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = null!;
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}
