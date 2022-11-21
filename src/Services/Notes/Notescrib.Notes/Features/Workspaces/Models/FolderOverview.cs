using Notescrib.Notes.Contracts;

namespace Notescrib.Notes.Features.Workspaces.Models;

public class FolderOverview : IChildrenCollectionTree<FolderOverview>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public ICollection<FolderOverview> Children { get; set; } = Array.Empty<FolderOverview>();
}
