using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Core.Api.Attributes;
using Notescrib.Notes.Api.Features.Notes.Models;
using Notescrib.Notes.Features.Notes.Commands;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Models;

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
    [ProducesResponseType(typeof(NoteOverview), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateNote(CreateNoteRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedList<NoteOverview>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotes([FromQuery] GetNotesRequest request, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToQuery(), cancellationToken));

    // [HttpGet("{id}")]
    // public async Task<IActionResult> GetNote(string id, CancellationToken cancellationToken)
    //     => Ok(await _mediator.Send(new GetNote.Query(id), cancellationToken));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(string id, UpdateNoteRequest request,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(id), cancellationToken));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(string id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new DeleteNote.Command(id), cancellationToken));

    [HttpPost("{id}/section")]
    public async Task<IActionResult> CreateNoteSection(string id, CreateNoteSectionRequest request,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(id), cancellationToken));

    [HttpPut("{id}/section/{sectionId}")]
    public async Task<IActionResult> UpdateNoteSection(
        string id,
        string sectionId,
        UpdateNoteSectionRequest request,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(id, sectionId), cancellationToken));

    [HttpDelete("{id}/section/{sectionId}")]
    public async Task<IActionResult> CreateNoteSection(string id, string sectionId,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new DeleteNoteSection.Command(id, sectionId), cancellationToken));
}
