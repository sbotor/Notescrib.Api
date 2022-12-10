using Notescrib.Notes.Features.Folders;

namespace Notescrib.Notes.Features.Workspaces;

public class Workspace
{
    public string Id { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public Folder FolderTree { get; set; } = null!;
    public int FolderCount { get; set; }
    public int NoteCount { get; set; }
    public DateTime Created { get; set; }
}

