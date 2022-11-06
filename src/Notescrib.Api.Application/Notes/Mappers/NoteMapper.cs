using Notescrib.Api.Application.Notes.Commands;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Notes.Mappers;

public interface INoteMapper
{
    Note MapToEntity(CreateNote.Command item, string ownerId, SharingInfo sharingInfo);
}

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
