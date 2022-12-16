using Notescrib.Notes.Api.Models;
using Notescrib.Notes.Features.Templates.Queries;

namespace Notescrib.Notes.Api.Features.Templates.Models;

public class SearchTemplatesRequest : PagingRequest
{
    public string? TextFilter { get; set; } = null!;

    public SearchNoteTemplates.Query ToQuery()
        => new(TextFilter, GetPaging());
}
