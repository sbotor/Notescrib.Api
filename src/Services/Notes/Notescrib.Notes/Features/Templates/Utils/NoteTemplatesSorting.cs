using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Templates.Utils;

public enum NoteTemplatesSorting
{
    Name,
    Updated,
    Created,
}

internal class NoteTemplatesSortingProvider : ISortingProvider<NoteTemplatesSorting>
{
    public string GetSortName(NoteTemplatesSorting value)
        => value switch
        {
            NoteTemplatesSorting.Name => nameof(NoteTemplate.Name),
            NoteTemplatesSorting.Updated => nameof(NoteTemplate.Updated),
            NoteTemplatesSorting.Created => nameof(NoteTemplate.Created),
            _ => throw new AppException(ErrorCodes.General.InvalidSortingProperty)
        };
}
