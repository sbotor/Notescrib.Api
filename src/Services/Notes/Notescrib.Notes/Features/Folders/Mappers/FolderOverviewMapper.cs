using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Folders.Models;

namespace Notescrib.Notes.Features.Folders.Mappers;

public class FolderOverviewMapper : IMapper<FolderData, FolderOverview>, IMapper<Folder, FolderOverview>
{
    public FolderOverview Map(FolderData item)
        => new() { Id = item.Id, Name = item.Name, Created = item.Created, Updated = item.Updated };

    public FolderOverview Map(Folder item)
        => Map((FolderData)item);
}
