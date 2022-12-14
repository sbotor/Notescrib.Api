using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Core.Api.Attributes;
using Notescrib.Notes.Api.Features.Notes.Models;
using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Notes.Commands;
using Notescrib.Notes.Features.Notes.Models;

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

    [HttpPost]
    public async Task<IActionResult> CreateNote(CreateNoteRequest request, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(), cancellationToken));
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NoteDetails), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNote(string id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetNote.Query(id), cancellationToken));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(string id, UpdateNoteRequest request,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(id), cancellationToken));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(string id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new DeleteNote.Command(id), cancellationToken));

    [HttpPut("content/{id}")]
    public async Task<IActionResult> UpdateContent(string id, string content, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new UpdateNoteContent.Command(id, content), cancellationToken));
}
