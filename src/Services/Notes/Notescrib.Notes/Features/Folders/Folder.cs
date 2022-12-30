using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Workspaces;

namespace Notescrib.Notes.Features.Folders;

public class FolderData
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

public class Folder : FolderData
{
    internal const string RootName = "*root";
    
    public Workspace Workspace { get; set; } = null!;
    public ICollection<Folder> ImmediateChildren { get; set; } = new List<Folder>();
    public ICollection<Folder> Children { get; set; } = new List<Folder>();
    public Folder? Parent { get; set; }
}
