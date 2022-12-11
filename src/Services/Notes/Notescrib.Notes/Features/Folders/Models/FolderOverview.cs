namespace Notescrib.Notes.Features.Folders.Models;

public class FolderOverview : FolderInfoBase
{
    public ICollection<FolderOverview> ChildrenIds { get; set; } = Array.Empty<FolderOverview>();
}
