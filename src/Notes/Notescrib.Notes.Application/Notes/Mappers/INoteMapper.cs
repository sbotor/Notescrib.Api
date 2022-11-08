using Notescrib.Notes.Application.Notes.Commands;
using Notescrib.Notes.Core.Entities;

namespace Notescrib.Notes.Application.Notes.Mappers;

public interface INoteMapper
{
    Note MapToEntity(CreateNote.Command item, string ownerId, SharingInfo sharingInfo);
}
