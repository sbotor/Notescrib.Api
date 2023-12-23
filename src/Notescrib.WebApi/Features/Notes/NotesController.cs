using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Features.Notes.Commands;
using Notescrib.Features.Notes.Models;
using Notescrib.Features.Notes.Queries;
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
    public async Task<IActionResult> Search([FromQuery] SearchNotesRequest request, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToQuery(), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> CreateNote(CreateNoteRequest request, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(), cancellationToken));

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(NoteDetails), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNote(Guid id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetNoteDetails.Query(id), cancellationToken));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateNote(Guid id, UpdateNoteRequest request,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(id), cancellationToken));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteNote(Guid id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new DeleteNote.Command(id), cancellationToken));

    [HttpPut("{id:guid}/content")]
    public async Task<IActionResult> UpdateContent(Guid id, UpdateContentRequest request,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(id), cancellationToken));

    [HttpPost("{id:guid}/related")]
    public async Task<IActionResult> UpdateRelated(Guid id, [FromBody] IEnumerable<Guid> relatedIds,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new AddRelatedNotes.Command(id, relatedIds), cancellationToken));

    [HttpDelete("{id:guid}/related")]
    public async Task<IActionResult> RemoveRelated(Guid id, [FromQuery] IEnumerable<Guid> relatedIds,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new RemoveRelatedNotes.Command(id, relatedIds), cancellationToken));
}
