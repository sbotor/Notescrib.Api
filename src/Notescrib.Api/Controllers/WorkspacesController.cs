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
    public async Task<IActionResult> ListWorkspaces([FromQuery] GetUserWorkspacesRequest request)
        => await GetResponseAsync(request.ToQuery());

    [HttpPost]
    [CreatedApiResponse]
    public async Task<IActionResult> AddWorkspace(AddWorkspaceRequest request)
        => await GetCreatedResponseAsync(request.ToCommand(), nameof(GetWorkspaceById));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWorkspace(string id, UpdateWorkspaceRequest request)
        => await GetResponseAsync(request.ToCommand(id));

    [HttpGet("{id}")]
    [ApiResponse(typeof(WorkspaceDetails))]
    [AllowAnonymous]
    public async Task<IActionResult> GetWorkspaceById(string id)
        => await GetResponseAsync(new GetWorkspaceById.Query(id));

    [HttpPost("{id}")]
    [CreatedApiResponse]
    public async Task<IActionResult> AddFolder(string id, AddFolderRequest request)
        => await GetCreatedResponseAsync(request.ToCommand(id), nameof(GetFolderDetails));

    [HttpGet("folder/{id}")]
    [ApiResponse(typeof(FolderDetails))]
    public Task<IActionResult> GetFolderDetails(string id)
        => Task.FromResult((IActionResult)StatusCode((int)HttpStatusCode.NotImplemented));

    [HttpPut("folder/{id}")]
    public async Task<IActionResult> UpdateFolder(string id, UpdateFolderRequest request)
        => await GetResponseAsync(request.ToCommand(id));
}
