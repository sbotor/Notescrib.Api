using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Folders.Models;
using Notescrib.Notes.Features.Workspaces;

namespace Notescrib.Notes.Features.Folders.Mappers;

public class FolderOverviewMapper : IMapper<Folder, FolderOverview>
{
    public FolderOverview Map(Folder item)
        => new()
        {
            Id = item.Id,
            Name = item.Name,
            ChildrenIds = new List<FolderOverview>(),
            Created = item.Created,
            Updated = item.Updated
        };
}
