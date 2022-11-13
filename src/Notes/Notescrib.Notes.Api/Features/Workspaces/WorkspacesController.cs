using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Notes.Api.Features.Workspaces.Models;
using Notescrib.Notes.Features.Workspaces.Models;
using Notescrib.Notes.Models;

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

    [HttpGet]
    [ProducesResponseType(typeof(PagedList<WorkspaceOverview>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserWorkspaces([FromQuery] GetWorkspacesRequest request)
        => Ok(await _mediator.Send(request.ToQuery()));
    
    [HttpPost]
    public async Task<IActionResult> CreateWorkspace(CreateWorkspaceRequest request)
        => Ok(await _mediator.Send(request.ToCommand()));

    [HttpPost("{id}")]
    public async Task<IActionResult> CreateFolder(string id, CreateFolderRequest request)
    {
        await _mediator.Send(request.ToCommand(id));
        return CreatedAtAction(nameof(GetUserWorkspaces), null);
    }
}
