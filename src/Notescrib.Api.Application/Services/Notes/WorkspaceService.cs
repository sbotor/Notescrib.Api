using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Notescrib.Api.Application.Contracts.Workspace;
using Notescrib.Api.Application.Mappers;
using Notescrib.Api.Application.Repositories;
using Notescrib.Api.Core;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Services.Notes;

internal class WorkspaceService : IWorkspaceService
{
    private const string WorkspaceNotFoundMsg = "Workspace with ID '{0}' not found.";
    private const string FolderNotFoundMsg = "Folder with path '{0}' not found.";

    private readonly IWorkspaceRepository _repository;
    private readonly IPermissionService _permissionService;
    private readonly IUserContextService _userContextService;
    private readonly IWorkspaceMapper _mapper;

    private readonly ILogger<WorkspaceService> _logger;

    public WorkspaceService(
        IWorkspaceRepository repository,
        IPermissionService permissionService,
        IUserContextService userContextService,
        IWorkspaceMapper mapper,
        ILogger<WorkspaceService> logger)
    {
        _repository = repository;
        _permissionService = permissionService;
        _userContextService = userContextService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<WorkspaceResponse>> AddWorkspaceAsync(WorkspaceRequest request)
    {
        if (string.IsNullOrEmpty(request.Name) || request.Name.Length < 4)
        {
            return ApiResponse<WorkspaceResponse>.Failure("Invalid workspace name.");
        }

        try
        {
            var ownerId = _userContextService.UserId;
            if (ownerId == null)
            {
                return ApiResponse<WorkspaceResponse>.Failure("No user context found.");
            }

            var workspace = _mapper.MapToEntity(request, ownerId);
            var added = await _repository.AddWorkspaceAsync(workspace);

            return ApiResponse<WorkspaceResponse>.Success(_mapper.MapToResponse(added));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating the workspace '{name}'.", request.Name);
            return ApiResponse<WorkspaceResponse>.Failure("Error creating the workspace.");
        }
    }

    public async Task<ApiResponse<WorkspaceResponse>> GetWorkspaceByIdAsync(string id)
    {
        var foundResponse = await GetWorkspaceInternal(id);
        if (!foundResponse.IsSuccessful || foundResponse.Response == null)
        {
            return foundResponse.CastError<WorkspaceResponse>();
        }

        var workspace = foundResponse.Response;

        if (!_permissionService.CanView(workspace.OwnerId, workspace.SharingDetails))
        {
            return ApiResponse<WorkspaceResponse>.Forbidden();
        }

        var response = _mapper.MapToResponse(workspace, null);
        return ApiResponse<WorkspaceResponse>.Success(response);
    }

    public async Task<ApiResponse<IReadOnlyCollection<WorkspaceResponse>>> ListUserWorkspacesAsync()
    {
        var ownerId = _userContextService.UserId;
        if (ownerId == null)
        {
            return ApiResponse<IReadOnlyCollection<WorkspaceResponse>>.Failure();
        }

        var result = await _repository.GetUserWorkspaces(ownerId);
        var response = result.Select(x => _mapper.MapToResponse(x)).ToList();

        return ApiResponse<IReadOnlyCollection<WorkspaceResponse>>.Success(response);
    }

    public async Task<ApiResponse<WorkspaceResponse>> UpdateWorkspace(string id, WorkspaceRequest request)
    {
        if (string.IsNullOrEmpty(request.Name) || request.Name.Length < 4)
        {
            return ApiResponse<WorkspaceResponse>.Failure("Invalid workspace name.");
        }

        request.SharingDetails ??= new();

        try
        {
            var foundResponse = await GetWorkspaceInternal(id);
            if (!foundResponse.IsSuccessful || foundResponse.Response == null)
            {
                return foundResponse.CastError<WorkspaceResponse>();
            }

            var workspace = foundResponse.Response;
            if (!_permissionService.CanEdit(workspace.OwnerId))
            {
                return ApiResponse<WorkspaceResponse>.Forbidden();
            }

            workspace = _mapper.MapToEntity(request, workspace.OwnerId, workspace);
            await _repository.UpdateWorkspaceAsync(workspace);

            var response = _mapper.MapToResponse(workspace, null);
            return ApiResponse<WorkspaceResponse>.Success(response);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating the workspace '{name}'.", request.Name);
            return ApiResponse<WorkspaceResponse>.Failure("Error updating the workspace.");
        }
    }

    public async Task<ApiResponse<string>> AddFolderAsync(FolderRequest request)
    {
        if (string.IsNullOrEmpty(request.ParentPath))
        {
            return ApiResponse<string>.Failure("Invalid parent path.");
        }

        if (string.IsNullOrEmpty(request.Name))
        {
            return ApiResponse<string>.Failure("Invalid folder name.");
        }

        var folder = new FolderPath
        {
            ParentPath = request.ParentPath,
            Name = request.Name
        };

        var workspace = await _repository.GetWorkspaceByIdAsync(folder.WorkspaceId);
        if (workspace == null)
        {
            return ApiResponse<string>.NotFound(string.Format(WorkspaceNotFoundMsg, folder.WorkspaceId));
        }

        if (!_permissionService.CanEdit(workspace.OwnerId))
        {
            return ApiResponse<string>.Forbidden();
        }

        if (workspace.Folders.Any(x => x.Name == folder.Name))
        {
            return ApiResponse<string>.Failure($"A folder with name '{folder.Name}' already exists.");
        }

        if (!workspace.Folders.Any(x => x.AbsolutePath == folder.ParentPath))
        {
            return ApiResponse<string>.NotFound(string.Format(FolderNotFoundMsg, folder.ParentPath));
        }

        workspace.Folders.Add(folder);
        await _repository.UpdateWorkspaceAsync(workspace);

        folder = workspace.Folders.First(x => x.Name == folder.Name);
        return ApiResponse<string>.Success(folder.AbsolutePath);
    }

    private async Task<ApiResponse<Workspace>> GetWorkspaceInternal(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return ApiResponse<Workspace>.Failure("The Id is empty.");
        }

        var found = await _repository.GetWorkspaceByIdAsync(id);

        return found != null
            ? ApiResponse<Workspace>.Success(found)
            : ApiResponse<Workspace>.NotFound();
    }
}
