using Notescrib.Api.Application.Contracts.Workspace;
using Notescrib.Api.Core;

namespace Notescrib.Api.Application.Services.Notes;

public interface IWorkspaceService
{
    Task<ApiResponse<string>> AddFolderAsync(FolderRequest request);
    Task<ApiResponse<WorkspaceResponse>> AddWorkspaceAsync(WorkspaceRequest request);
    Task<ApiResponse<WorkspaceResponse>> GetWorkspaceByIdAsync(string id);
    Task<ApiResponse<IReadOnlyCollection<WorkspaceResponse>>> ListUserWorkspacesAsync();
    Task<ApiResponse<WorkspaceResponse>> UpdateWorkspace(string id, WorkspaceRequest request);
}
