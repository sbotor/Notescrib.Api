using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces;

public interface IWorkspaceRepository
{
    Task<Workspace> AddWorkspaceAsync(Workspace workspace);
    Task<bool> DeleteWorkspaceAsync(string workspaceId);
    Task<ICollection<Workspace>> GetUserWorkspaces(string ownerId);
    Task<Workspace?> GetWorkspaceByIdAsync(string workspaceId);
    Task UpdateWorkspaceAsync(Workspace workspace);
}
