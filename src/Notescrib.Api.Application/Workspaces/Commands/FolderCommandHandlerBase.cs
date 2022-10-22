using AutoMapper;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

internal abstract class FolderCommandHandlerBase
{
    protected IWorkspaceRepository WorkspaceRepository { get; }
    protected IPermissionService PermissionService { get; }

    public FolderCommandHandlerBase(IWorkspaceRepository repository, IPermissionService permissionService)
    {
        WorkspaceRepository = repository;
        PermissionService = permissionService;
    }

    protected async Task<Result<Workspace>> ValidateAndGetWorkspace(Folder folder)
    {
        var workspace = await WorkspaceRepository.GetWorkspaceByIdAsync(folder.WorkspaceId);
        if (workspace == null)
        {
            return Result<Workspace>.NotFound($"Workspace with ID '{folder.WorkspaceId}' not found.");
        }

        if (!PermissionService.CanEdit(workspace))
        {
            return Result<Workspace>.Forbidden();
        }

        if (workspace.Folders.Any(x => x.Name == folder.Name))
        {
            return Result<Workspace>.Failure($"A folder with name '{folder.Name}' already exists.");
        }

        if (folder.ParentPath != null
            && !workspace.Folders.Any(x => x.AbsolutePath == folder.ParentPath))
        {
            return Result<Workspace>.NotFound($"Folder with path '{folder.ParentPath}' not found.");
        }

        return Result<Workspace>.Success(workspace);
    }
}
