using Notescrib.Contracts;
using Notescrib.Features.Folders.Models;

namespace Notescrib.Features.Folders.Mappers;

public class FolderOverviewMapper : IMapper<Folder, FolderOverview>
{
    public FolderOverview Map(Folder item)
        => new() { Id = item.Id, Name = item.Name, Created = item.Created, Updated = item.Updated };
}
