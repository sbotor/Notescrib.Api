using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Repositories;

public interface IWorkspaceRepository
{
    Task<Workspace> AddWorkspaceAsync(Workspace workspace);
    Task<bool> DeleteWorkspaceAsync(string workspaceId);
    Task<Workspace?> GetWorkspaceByIdAsync(string workspaceId);
    Task<Workspace> UpdateWorkspaceAsync(Workspace workspace);
}
