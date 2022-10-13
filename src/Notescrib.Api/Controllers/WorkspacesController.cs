using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Api.Application.Workspaces.Queries;
using Notescrib.Api.Contracts.Workspaces;

namespace Notescrib.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class WorkspacesController : ApiControllerBase
{
    public WorkspacesController(ISender mediator) : base(mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> ListWorkspaces()
        => await GetResponseAsync(new GetUserWorkspaces.Query());

    [HttpPost]
    public async Task<IActionResult> AddWorkspace(AddWorkspaceRequest request)
        => await GetResponseAsync(request.ToCommand());

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWorkspace(string id, UpdateWorkspaceRequest request)
        => await GetResponseAsync(request.ToCommand(id));
}
