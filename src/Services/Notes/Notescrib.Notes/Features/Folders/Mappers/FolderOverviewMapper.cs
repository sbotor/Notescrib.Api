using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Folders.Models;
using Notescrib.Notes.Features.Workspaces;

namespace Notescrib.Notes.Features.Folders.Mappers;

public class FolderOverviewMapper : IMapper<FolderData, FolderOverview>
{
    public FolderOverview Map(FolderData item)
        => new() { Id = item.Id, Name = item.Name, Created = item.Created, Updated = item.Updated };
}
