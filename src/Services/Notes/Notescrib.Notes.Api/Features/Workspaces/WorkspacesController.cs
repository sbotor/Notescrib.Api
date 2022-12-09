using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Notes.Features.Workspaces.Commands;

namespace Notescrib.Notes.Api.Features.Workspaces;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkspacesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkspacesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateWorkspace(CancellationToken cancellationToken)
        => StatusCode(
            StatusCodes.Status201Created,
            await _mediator.Send(new CreateWorkspace.Command(), cancellationToken));
}
