using Notescrib.Features.Notes.Queries;
using Notescrib.WebApi.Models;

namespace Notescrib.WebApi.Features.Notes.Models;

public class SearchNotesRequest : PagingRequest
{
    public string? TextFilter { get; set; }
    public bool OwnOnly { get; set; }

    public SearchNotes.Query ToQuery()
        => new(TextFilter, OwnOnly, GetPaging());
}
