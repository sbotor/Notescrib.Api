using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Notes.Api.Features.Workspaces.Models;

namespace Notescrib.Notes.Api.Features.Workspaces;

[ApiController]
[Route("api/[controller]")]
public class WorkspacesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkspacesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserWorkspaces([FromQuery] GetWorkspacesRequest request)
        => Ok(await _mediator.Send(request.ToQuery()));
}
