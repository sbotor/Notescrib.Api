using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Features.Folders.Commands;
using Notescrib.Features.Folders.Models;
using Notescrib.Features.Folders.Queries;
using Notescrib.WebApi.Features.Folders.Models;

namespace Notescrib.WebApi.Features.Folders;

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
    public Task CreateFolder(CreateFolderRequest request,
        CancellationToken cancellationToken)
        => _mediator.Send(request.ToCommand(), cancellationToken);

    [HttpPut("{id:guid}")]
    public Task UpdateFolder(
        Guid id,
        UpdateFolderRequest request,
        CancellationToken cancellationToken)
        =>  _mediator.Send(request.ToCommand(id), cancellationToken);
    
    [HttpDelete("{id:guid}")]
    public Task DeleteFolder(
        Guid id,
        CancellationToken cancellationToken)
        => _mediator.Send(new DeleteFolder.Command(id), cancellationToken);

    [HttpGet("{id:guid?}")]
    [ProducesResponseType(typeof(FolderDetails), StatusCodes.Status200OK)]
    public Task<FolderDetails> GetFolderDetails(Guid? id, CancellationToken cancellationToken)
        => _mediator.Send(new GetFolderDetails.Query(id), cancellationToken);
}
