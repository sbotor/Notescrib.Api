using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Api.Application.Workspaces.Contracts;
using Notescrib.Api.Application.Workspaces.Queries;
using Notescrib.Api.Attributes;
using Notescrib.Api.Contracts.Workspaces;

namespace Notescrib.Api.Controllers;

[ControllerRoute]
[Authorize]
public class WorkspacesController : ApiControllerBase
{
    public WorkspacesController(ISender mediator) : base(mediator)
    {
    }

    [HttpGet]
    [ApiResponse(typeof(IReadOnlyCollection<WorkspaceResponse>))]
    public async Task<IActionResult> ListWorkspaces()
        => await GetResponseAsync(new GetUserWorkspaces.Query());

    [HttpPost]
    [ApiResponse(typeof(WorkspaceResponse), HttpStatusCode.Created)]
    public async Task<IActionResult> AddWorkspace(AddWorkspaceRequest request)
        => await GetResponseAsync(request.ToCommand());

    [HttpPut("{id}")]
    [ApiResponse(typeof(void))]
    public async Task<IActionResult> UpdateWorkspace(string id, UpdateWorkspaceRequest request)
        => await GetResponseAsync(request.ToCommand(id));
}
