using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Notes.Api.Features.Folders.Models;
using Notescrib.Notes.Features.Folders.Commands;
using Notescrib.Notes.Features.Folders.Models;
using Notescrib.Notes.Features.Folders.Queries;

namespace Notescrib.Notes.Api.Features.Folders;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FoldersController : ControllerBase
{
    private readonly IMediator _mediator;

    public FoldersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(FolderInfoBase), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateFolder(CreateFolderRequest request,
        CancellationToken cancellationToken)
        => StatusCode(
            StatusCodes.Status201Created,
            await _mediator.Send(request.ToCommand(), cancellationToken));

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFolder(
        string id,
        UpdateFolderRequest request,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(id), cancellationToken));
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteFolder(
        string id,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new DeleteFolder.Command(id), cancellationToken));

    [HttpGet("{id?}")]
    [ProducesResponseType(typeof(FolderDetails), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFolderDetails(string? id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetFolderDetails.Query(id), cancellationToken));
}
