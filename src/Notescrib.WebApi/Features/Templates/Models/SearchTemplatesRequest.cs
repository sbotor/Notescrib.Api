using Notescrib.Features.Templates.Queries;
using Notescrib.WebApi.Models;

namespace Notescrib.WebApi.Features.Templates.Models;

public class SearchTemplatesRequest : PagingRequest
{
    public string? TextFilter { get; set; } = null!;

    public SearchNoteTemplates.Query ToQuery()
        => new(TextFilter, GetPaging());
}
