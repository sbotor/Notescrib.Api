using Notescrib.Api.Application.Contracts.Workspace;
using Notescrib.Api.Core;

namespace Notescrib.Api.Application.Services;
internal interface IWorkspaceService
{
    Task<ApiResponse<string>> AddFolderAsync(string parentPath, string folderName);
    Task<ApiResponse<WorkspaceResponse>> AddWorkspace(WorkspaceRequest request);
    Task<ApiResponse<WorkspaceResponse>> GetWorkspaceByIdAsync(string id);
    Task<ApiResponse<WorkspaceResponse>> UpdateWorkspace(string id, WorkspaceRequest request);
}