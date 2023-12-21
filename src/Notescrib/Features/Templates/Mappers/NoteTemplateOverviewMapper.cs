using Notescrib.Contracts;
using Notescrib.Features.Templates.Models;

namespace Notescrib.Features.Templates.Mappers;

public class NoteTemplateOverviewMapper : IMapper<NoteTemplate, NoteTemplateOverview>
{
    public NoteTemplateOverview Map(NoteTemplate item)
        => new()
        {
            Id = item.Id,
            Name = item.Name,
            OwnerId = item.OwnerId,
            Created = item.Created,
            Updated = item.Updated
        };
}
