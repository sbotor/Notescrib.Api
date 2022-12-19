using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Notes.Models;

namespace Notescrib.Notes.Features.Notes.Mappers;

public class NoteOverviewMapper : IMapper<NoteData, NoteOverview>, IMapper<Note, NoteOverview>
{
    public NoteOverview Map(NoteData item)
        => new()
        {
            Id = item.Id,
            Name = item.Name,
            FolderId = item.FolderId,
            OwnerId = item.OwnerId,
            SharingInfo = item.SharingInfo,
            Updated = item.Updated,
            Created = item.Created,
            Tags = item.Tags.ToArray()
        };

    public NoteOverview Map(Note item)
        => Map((NoteData)item);
}
