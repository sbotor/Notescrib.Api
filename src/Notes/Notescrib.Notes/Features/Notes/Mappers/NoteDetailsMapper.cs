using Notescrib.Notes.Contracts;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Notes.Models;

namespace Notescrib.Notes.Features.Notes.Mappers;

public class NoteDetailsMapper : IMapper<Note, NoteDetails>
{
    public NoteDetails Map(Note item)
        => new()
        {
            Id = item.Id,
            Name = item.Name,
            WorkspaceId = item.WorkspaceId,
            Folder = item.Folder,
            Labels = item.Labels,
            OwnerId = item.OwnerId,
            SharingInfo = item.SharingInfo,
            Contents = item.Contents.MapTree(x => new NoteContentsSection
            {
                Name = x.Name,
                Content = x.Content,
                Children = new List<NoteContentsSection>()
            }).ToArray()
        };
}
