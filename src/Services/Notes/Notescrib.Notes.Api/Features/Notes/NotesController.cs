using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Core.Api.Attributes;
using Notescrib.Notes.Api.Features.Notes.Models;
using Notescrib.Notes.Features.Notes.Commands;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Features.Notes.Queries;

namespace Notescrib.Notes.Api.Features.Notes;

[ApiController]
[ApiRoute]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] SearchNotesRequest request, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToQuery(), cancellationToken));
    
    [HttpPost]
    public async Task<IActionResult> CreateNote(CreateNoteRequest request, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(), cancellationToken));
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NoteDetails), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNote(string id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetNoteDetails.Query(id), cancellationToken));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(string id, UpdateNoteRequest request,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(id), cancellationToken));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(string id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new DeleteNote.Command(id), cancellationToken));

    [HttpPut("{id}/content")]
    public async Task<IActionResult> UpdateContent(string id, UpdateContentRequest request, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(id), cancellationToken));
}
