using Notescrib.Contracts;
using Notescrib.Features.Folders;
using Notescrib.Models;

namespace Notescrib.Features.Notes;

public class NoteBase : IShareable
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string WorkspaceId { get; set; } = null!;
    public string FolderId { get; set; } = null!;
    public ICollection<string> Tags { get; set; } = new List<string>();
    public ICollection<string> RelatedIds { get; set; } = new List<string>();
    public string OwnerId { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = null!;
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}

public class NoteData : NoteBase
{
    public string Content { get; set; } = string.Empty;
}

public class Note : NoteData
{
    public Folder Folder { get; set; } = null!;
    public IReadOnlyCollection<NoteBase> Related { get; set; } = null!;
}
