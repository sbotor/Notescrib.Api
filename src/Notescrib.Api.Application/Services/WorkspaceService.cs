using System.Net;
using Microsoft.Extensions.Logging;
using Notescrib.Api.Application.Contracts.Workspace;
using Notescrib.Api.Application.Mappers;
using Notescrib.Api.Application.Repositories;
using Notescrib.Api.Core;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Services;

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

    public async Task<ApiResponse<WorkspaceResponse>> AddWorkspace(WorkspaceRequest request)
    {
        if (string.IsNullOrEmpty(request.Name) || request.Name.Length < 4)
        {
            return ApiResponse<WorkspaceResponse>.Failure("Invalid workspace name.");
        }

        try
        {
            var workspace = _mapper.MapToEntity(request, _userContextService.UserId);

            var added = await _repository.AddWorkspaceAsync(new Workspace
            {
                Name = request.Name
            });

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

            var updated = await _repository.UpdateWorkspaceAsync(
                _mapper.MapToEntity(request, _userContextService.UserId, workspace));

            var response = _mapper.MapToResponse(workspace, null);
            return ApiResponse<WorkspaceResponse>.Success(response);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating the workspace '{name}'.", request.Name);
            return ApiResponse<WorkspaceResponse>.Failure("Error updating the workspace.");
        }
    }

    public async Task<ApiResponse<string>> AddFolderAsync(string parentPath, string folderName)
    {
        var folder = new FolderPath
        {
            ParentPath = parentPath,
            Name = folderName
        };

        var workspace = await _repository.GetWorkspaceByIdAsync(folder.WorkspaceId);
        if (workspace == null)
        {
            return ApiResponse<string>.Failure(string.Format(WorkspaceNotFoundMsg, folder.WorkspaceId), HttpStatusCode.NotFound);
        }

        if (_permissionService.CanEdit(workspace.OwnerId))
        {
            return ApiResponse<string>.Forbidden();
        }

        if (workspace.Folders.Any(x => x.Name == folder.Name))
        {
            return ApiResponse<string>.Failure($"A folder with name '{folder.Name}' already exists.");
        }

        if (!workspace.Folders.Any(x => x.AbsolutePath == folder.ParentPath))
        {
            return ApiResponse<string>.Failure(string.Format(FolderNotFoundMsg, folder.ParentPath), HttpStatusCode.NotFound);
        }

        workspace.Folders.Add(folder);
        workspace = await _repository.UpdateWorkspaceAsync(workspace);

        folder = workspace.Folders.First(x => x.Name == folderName);
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
