using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Models;

namespace Notescrib.Notes.Features.Notes;

public class NoteData : IShareable
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string WorkspaceId { get; set; } = null!;
    public string FolderId { get; set; } = null!;
    public ICollection<string> Tags { get; set; } = new List<string>();
    public string Content { get; set; } = string.Empty;
    public string OwnerId { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = null!;
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}

public class Note : NoteData
{
    public Folder Folder { get; set; } = null!;
}
