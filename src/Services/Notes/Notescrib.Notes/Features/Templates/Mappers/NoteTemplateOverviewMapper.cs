using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Templates.Models;

namespace Notescrib.Notes.Features.Templates.Mappers;

public class NoteTemplateOverviewMapper : IMapper<NoteTemplateBase, NoteTemplateOverview>
{
    public NoteTemplateOverview Map(NoteTemplateBase item)
        => new()
        {
            Id = item.Id,
            Name = item.Name,
            OwnerId = item.OwnerId,
            WorkspaceId = item.WorkspaceId,
            Created = item.Created,
            Updated = item.Updated
        };
}
