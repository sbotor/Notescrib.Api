using Notescrib.Notes.Contracts;

namespace Notescrib.Notes.Features.Folders.Models;

public class FolderOverview : FolderInfoBase, IChildrenCollectionTree<FolderOverview>
{
    public ICollection<FolderOverview> Children { get; set; } = Array.Empty<FolderOverview>();
}
