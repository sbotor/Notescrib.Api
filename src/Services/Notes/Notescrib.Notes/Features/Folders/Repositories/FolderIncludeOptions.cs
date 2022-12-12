namespace Notescrib.Notes.Features.Folders.Repositories;

public class FolderIncludeOptions
{
    public bool Workspace { get; set; }
    
    [Obsolete("Find another way to get all children notes.")]
    public bool ChildrenNotes { get; set; }
    public bool ImmediateNotes { get; set; }
    public bool ImmediateChildren { get; set; }
    public bool Children { get; set; }
    public bool Parent { get; set; }
}
