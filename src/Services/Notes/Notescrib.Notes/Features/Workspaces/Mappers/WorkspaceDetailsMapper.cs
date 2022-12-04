using Notescrib.Notes.Contracts;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Workspaces.Models;

namespace Notescrib.Notes.Features.Workspaces.Mappers;

public class WorkspaceDetailsMapper : IMapper<Workspace, WorkspaceDetails>, IMapper<Folder, FolderOverview>
{
    public WorkspaceDetails Map(Workspace item)
        => new()
        {
            Id = item.Id,
            Name = item.Name,
            OwnerId = item.OwnerId,
            FolderTree = MapFolders(item.Folders),
            Created = item.Created,
            Updated = item.Updated
        };

    private IReadOnlyCollection<FolderOverview> MapFolders(IEnumerable<Folder> source)
        => source.MapTree(Map, new Comparer()).ToList();

    public FolderOverview Map(Folder item)
        => new()
        {
            Id = item.Id,
            Name = item.Name,
            Children = new List<FolderOverview>(),
            Created = item.Created,
            Updated = item.Updated
        };

    private class Comparer : IComparer<Folder>
    {
        public int Compare(Folder? x, Folder? y)
            => string.CompareOrdinal(x?.Name.ToLowerInvariant(), y?.Name.ToLowerInvariant());
    }
}
