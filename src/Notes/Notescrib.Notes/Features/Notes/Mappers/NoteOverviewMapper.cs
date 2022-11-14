using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Notes.Models;

namespace Notescrib.Notes.Features.Notes.Mappers;

public class NoteOverviewMapper : IMapper<Note, NoteOverview>
{
    public NoteOverview Map(Note item)
        => new()
        {
            Id = item.Id,
            Name = item.Name,
            WorkspaceId = item.WorkspaceId,
            Folder = item.Folder,
            OwnerId = item.OwnerId,
            SharingInfo = item.SharingInfo
        };
}
