using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Features.Workspaces.Commands;

namespace Notescrib.WebApi.Features.Workspaces;

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
    public async Task<IActionResult> CreateWorkspace(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new CreateWorkspace.Command(), cancellationToken));

    [HttpDelete]
    public async Task<IActionResult> DeleteWorkspace(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new DeleteWorkspace.Command(), cancellationToken));
}
