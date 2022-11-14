using Notescrib.Notes.Api.Models;
using Notescrib.Notes.Features.Notes.Commands;
using Notescrib.Notes.Features.Notes.Utils;

namespace Notescrib.Notes.Api.Features.Notes.Models;

public class GetNotesRequest : PagingSortingRequest<NotesSorting>
{
    public string? WorkspaceId { get; set; }
    public string? Folder { get; set; }

    public GetNotes.Query ToQuery()
        => new(
            WorkspaceId,
            Folder,
            new(Page, PageSize),
            new(OrderBy, SortingDirection));
}
