namespace Notescrib.Features.Folders.Repositories;

public class FolderIncludeOptions
{
    public bool Workspace { get; set; }
    public bool ImmediateChildren { get; set; }
    public bool Children { get; set; }
    public bool Parent { get; set; }
}
