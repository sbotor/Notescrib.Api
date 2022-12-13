using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Workspaces;

namespace Notescrib.Notes.Features.Folders;

public class FolderBase
{
    public string Id { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    
    public ICollection<string> AncestorIds { get; set; } = new List<string>();
    public string? ParentId { get; set; }
    public string WorkspaceId { get; set; } = null!;
}

public class Folder : FolderBase
{
    public Workspace Workspace { get; set; } = null!;
    public IReadOnlyCollection<Note> ChildrenNotes { get; set; } = null!;
    public IReadOnlyCollection<Note> ImmediateNotes { get; set; } = null!;
    public IReadOnlyCollection<Folder> ImmediateChildren { get; set; } = null!;
    public IReadOnlyCollection<Folder> Children { get; set; } = null!;
    public Folder? Parent { get; set; } = null!;
}
