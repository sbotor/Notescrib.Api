using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Core.Api.Attributes;
using Notescrib.Notes.Api.Features.Notes.Models;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Features.Notes.Queries;
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
    public async Task<IActionResult> GetNotes([FromQuery] GetNotesRequest request)
        => Ok(await _mediator.Send(request.ToQuery()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNote(string id)
        => Ok(await _mediator.Send(new GetNote.Query(id)));

    [HttpPut("{id}/contents")]
    public async Task<IActionResult> UpdateNoteContents(string id, UpdateNoteContentsRequest request)
        => Ok(await _mediator.Send(request.ToCommand(id)));
}
