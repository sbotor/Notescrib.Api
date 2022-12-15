using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Notes.Models;

namespace Notescrib.Notes.Features.Notes.Mappers;

public class NoteDetailsMapper : IMapper<NoteContent, NoteDetails>
{
    public NoteDetails Map(NoteContent item)
    {
        var note = item.Note;

        return new()
        {
            Id = note.Id,
            Name = note.Name,
            FolderId = note.FolderId,
            OwnerId = note.OwnerId,
            SharingInfo = note.SharingInfo,
            Updated = note.Updated,
            Created = note.Created,
            Tags = note.Tags.ToArray(),
            Content = item.Value
        };
    }
}
