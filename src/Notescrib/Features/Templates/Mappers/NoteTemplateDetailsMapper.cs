using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Templates.Models;

namespace Notescrib.Notes.Features.Templates.Mappers;

public class NoteTemplateDetailsMapper : IMapper<NoteTemplate, NoteTemplateDetails>
{
    public NoteTemplateDetails Map(NoteTemplate item)
        => new()
        {
            Id = item.Id,
            Name = item.Name,
            OwnerId = item.OwnerId,
            Created = item.Created,
            Updated = item.Updated,
            Content = item.Content
        };
}
