using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Application.Workspaces.Queries;
using Notescrib.Api.Attributes;
using Notescrib.Api.Contracts.Workspaces;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Controllers;

[ControllerRoute]
[Authorize]
public class WorkspacesController : ApiControllerBase
{
    public WorkspacesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    [ApiResponse(typeof(PagedList<WorkspaceDetails>))]
    public async Task<IActionResult> ListWorkspaces([FromQuery] GetUserWorkspacesRequest request)
        => await GetResponseAsync(request.ToQuery());

    [HttpPost]
    [ApiResponse(typeof(WorkspaceDetails), HttpStatusCode.Created)]
    public async Task<IActionResult> AddWorkspace(AddWorkspaceRequest request)
        => await GetResponseAsync(request.ToCommand());

    [HttpPut("{id}")]
    [ApiResponse(typeof(void))]
    public async Task<IActionResult> UpdateWorkspace(string id, UpdateWorkspaceRequest request)
        => await GetResponseAsync(request.ToCommand(id));

    [HttpGet("{id}")]
    [ApiResponse(typeof(WorkspaceDetails))]
    [AllowAnonymous]
    public async Task<IActionResult> GetWorkspaceById(string id)
        => await GetResponseAsync(new GetWorkspaceById.Query(id));

    [HttpPost("{id}/folders")]
    [ApiResponse(typeof(void))]
    public async Task<IActionResult> AddFolder(string id, AddFolderRequest request)
        => await GetResponseAsync(request.ToCommand(id));
}
