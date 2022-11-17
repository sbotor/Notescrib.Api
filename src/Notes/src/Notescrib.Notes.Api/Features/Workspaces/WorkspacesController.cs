using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Notes.Api.Features.Workspaces.Models;
using Notescrib.Notes.Features.Workspaces.Commands;
using Notescrib.Notes.Features.Workspaces.Models;
using Notescrib.Notes.Features.Workspaces.Queries;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserWorkspaces([FromQuery] GetWorkspacesRequest request, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToQuery(), cancellationToken));
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateWorkspace(CreateWorkspaceRequest request, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(), cancellationToken));

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateWorkspace(
        string id,
        UpdateWorkspaceRequest request,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(id), cancellationToken));

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WorkspaceDetails), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWorkspace(string id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetWorkspaceDetails.Query(id), cancellationToken));
    
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(WorkspaceDetails), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteWorkspace(string id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new DeleteWorkspace.Command(id), cancellationToken));
    
    [HttpPost("{id}/folder")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateFolder(string id, CreateFolderRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(request.ToCommand(id), cancellationToken);
        return CreatedAtAction(nameof(GetUserWorkspaces), null);
    }
    
    [HttpPut("{id}/folder/{folderId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFolder(
        string id,
        string folderId,
        UpdateFolderRequest request,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(id, folderId), cancellationToken));
    
    [HttpDelete("{id}/folder/{folderId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteFolder(
        string id,
        string folderId,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new DeleteFolder.Command(id, folderId), cancellationToken));
}
