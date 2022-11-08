using Notescrib.Notes.Application.Notes.Commands;
using Notescrib.Notes.Core.Entities;

namespace Notescrib.Notes.Application.Notes.Mappers;

public class NoteMapper : INoteMapper
{
    public Note MapToEntity(CreateNote.Command item, string ownerId, SharingInfo sharingInfo)
        => new()
        {
            Name = item.Name,
            FolderId = item.FolderId,
            Labels = item.Labels.ToArray(),
            OwnerId = ownerId,
            SharingInfo = sharingInfo
        };
}
