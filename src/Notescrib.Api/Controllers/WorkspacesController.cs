using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Api.Application.Contracts.Workspace;
using Notescrib.Api.Application.Services.Notes;

namespace Notescrib.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class WorkspacesController : ApiControllerBase
{
    private readonly IWorkspaceService _workspaceService;

    public WorkspacesController(IWorkspaceService workspaceService)
    {
        _workspaceService = workspaceService;
    }

    [HttpGet()]
    public async Task<IActionResult> ListWorkspaces()
        => GetResult(await _workspaceService.ListUserWorkspacesAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWorkspaceById(string id)
        => GetResult(await _workspaceService.GetWorkspaceByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> AddWorkspace(WorkspaceRequest request)
        => GetResult(await _workspaceService.AddWorkspaceAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> EditWorkspace(string id, WorkspaceRequest request)
        => GetResult(await _workspaceService.UpdateWorkspace(id, request));

    [HttpPost("folder")]
    public async Task<IActionResult> AddFolder(FolderRequest request)
        => GetResult(await _workspaceService.AddFolderAsync(request));
}
