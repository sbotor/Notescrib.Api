using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Application.Workspaces.Queries;
using Notescrib.Api.Attributes;
using Notescrib.Api.Contracts.Workspaces;
using Notescrib.Api.Core.Contracts;

namespace Notescrib.Api.Controllers;

[ControllerRoute]
[Authorize]
public class WorkspacesController : ApiControllerBase
{
    public WorkspacesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    [ApiResponse(typeof(IPagedList<WorkspaceOverview>))]
    public Task<IActionResult> ListWorkspaces([FromQuery] GetUserWorkspacesRequest request)
        => Ok(request.ToQuery());

    [HttpPost]
    [CreatedApiResponse]
    public Task<IActionResult> AddWorkspace(AddWorkspaceRequest request)
        => CreatedAtAction(request.ToCommand(), nameof(GetWorkspaceById));

    [HttpPut("{id}")]
    public Task<IActionResult> UpdateWorkspace(string id, UpdateWorkspaceRequest request)
        => Ok(request.ToCommand(id));

    [HttpGet("{id}")]
    [ApiResponse(typeof(WorkspaceDetails))]
    [AllowAnonymous]
    public Task<IActionResult> GetWorkspaceById(string id)
        => Ok(new GetWorkspaceById.Query(id));

    [HttpPost("{id}")]
    [CreatedApiResponse]
    public Task<IActionResult> AddFolder(string id, AddFolderRequest request)
        => CreatedAtAction(request.ToCommand(id), nameof(GetFolderDetails));

    [HttpGet("folder/{id}")]
    [ApiResponse(typeof(FolderDetails))]
    public Task<IActionResult> GetFolderDetails(string id)
        => Task.FromResult((IActionResult)StatusCode((int)HttpStatusCode.NotImplemented));

    [HttpPut("folder/{id}")]
    public Task<IActionResult> UpdateFolder(string id, UpdateFolderRequest request)
        => Ok(request.ToCommand(id));
}
