using Notescrib.Notes.Models;

namespace Notescrib.Notes.Features.Notes;

public class Note
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string WorkspaceId { get; set; } = null!;
    public string? Folder { get; set; }
    public NoteContent Content { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = null!;
}

public class NoteContent
{
    public NoteSection? RootSection { get; set; } = null!;
}

public class NoteSection
{
    public string Name { get; set; } = null!;
    public string Content { get; set; } = null!;
    public ICollection<NoteSection> Children { get; set; } = new List<NoteSection>();
}
