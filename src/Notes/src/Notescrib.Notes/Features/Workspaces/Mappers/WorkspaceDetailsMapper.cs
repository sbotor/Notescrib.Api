using Notescrib.Notes.Contracts;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Workspaces.Models;
using Notescrib.Notes.Utils.Tree;

namespace Notescrib.Notes.Features.Workspaces.Mappers;

public class WorkspaceDetailsMapper : IMapper<Workspace, WorkspaceDetails>
{
    public WorkspaceDetails Map(Workspace item)
        => new()
        {
            Id = item.Id,
            Name = item.Name,
            OwnerId = item.OwnerId,
            FolderTree = MapFolders(item.Folders)
        };

    private static IReadOnlyCollection<FolderOverview> MapFolders(IEnumerable<Folder> source)
        => source.MapTree(MapFolderOverview).ToList();

    private static FolderOverview MapFolderOverview(Folder item)
        => new() { Id = item.Id, Name = item.Name, Children = new List<FolderOverview>() };
}
