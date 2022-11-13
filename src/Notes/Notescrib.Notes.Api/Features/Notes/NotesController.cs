using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Core.Api.Attributes;
using Notescrib.Notes.Api.Features.Notes.Models;

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
    public async Task<IActionResult> CreateNote(CreateNoteRequest request)
    {
        var result = await _mediator.Send(request.ToCommand());
        return Ok(result);
    }
}
