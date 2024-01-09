using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Features.Notes.Commands;
using Notescrib.Features.Notes.Models;
using Notescrib.Features.Notes.Queries;
using Notescrib.Models;
using Notescrib.WebApi.Features.Notes.Models;

namespace Notescrib.WebApi.Features.Notes;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public Task<PagedList<NoteOverview>> Search([FromQuery] SearchNotesRequest request, CancellationToken cancellationToken)
        => _mediator.Send(request.ToQuery(), cancellationToken);

    [HttpPost]
    public Task CreateNote(CreateNoteRequest request, CancellationToken cancellationToken)
        => _mediator.Send(request.ToCommand(), cancellationToken);

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(NoteDetails), StatusCodes.Status200OK)]
    public Task<NoteDetails> GetNote(Guid id, CancellationToken cancellationToken)
        => _mediator.Send(new GetNoteDetails.Query(id), cancellationToken);

    [HttpPut("{id:guid}")]
    public Task UpdateNote(Guid id, UpdateNoteRequest request,
        CancellationToken cancellationToken)
        => _mediator.Send(request.ToCommand(id), cancellationToken);

    [HttpDelete("{id:guid}")]
    public Task DeleteNote(Guid id, CancellationToken cancellationToken)
        => _mediator.Send(new DeleteNote.Command(id), cancellationToken);

    [HttpPut("{id:guid}/content")]
    public Task UpdateContent(Guid id, UpdateContentRequest request,
        CancellationToken cancellationToken)
        => _mediator.Send(request.ToCommand(id), cancellationToken);

    [HttpPost("{id:guid}/related")]
    public Task UpdateRelated(Guid id, [FromBody] IEnumerable<Guid> relatedIds,
        CancellationToken cancellationToken)
        => _mediator.Send(new AddRelatedNotes.Command(id, relatedIds), cancellationToken);

    [HttpDelete("{id:guid}/related")]
    public Task RemoveRelated(Guid id, [FromQuery] IEnumerable<Guid> relatedIds,
        CancellationToken cancellationToken)
        => _mediator.Send(new RemoveRelatedNotes.Command(id, relatedIds), cancellationToken);
}
