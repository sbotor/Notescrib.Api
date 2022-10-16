using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces;

public interface IWorkspaceRepository
{
    Task<Workspace> AddWorkspaceAsync(Workspace workspace);
    Task<bool> DeleteWorkspaceAsync(string workspaceId);
    Task<PagedList<Workspace>> GetUserWorkspacesAsync(string ownerId, int pageNumber, int pageSize);
    Task<Workspace?> GetWorkspaceByIdAsync(string workspaceId);
    Task UpdateWorkspaceAsync(Workspace workspace);
}
