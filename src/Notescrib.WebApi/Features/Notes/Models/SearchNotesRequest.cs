using Notescrib.Notes.Api.Models;
using Notescrib.Notes.Features.Notes.Queries;

namespace Notescrib.Notes.Api.Features.Notes.Models;

public class SearchNotesRequest : PagingRequest
{
    public string? TextFilter { get; set; }
    public bool OwnOnly { get; set; }

    public SearchNotes.Query ToQuery()
        => new(TextFilter, OwnOnly, GetPaging());
}
