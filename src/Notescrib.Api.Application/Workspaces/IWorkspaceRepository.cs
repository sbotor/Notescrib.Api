using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces;

public interface IWorkspaceRepository
{
    Task<string> AddWorkspaceAsync(Workspace workspace);
    Task DeleteWorkspaceAsync(string workspaceId);
    Task<IPagedList<Workspace>> GetUserWorkspacesAsync(string ownerId, IPaging paging, ISorting? sorting = null);
    Task<Workspace?> GetWorkspaceByIdAsync(string workspaceId);
    Task UpdateWorkspaceAsync(Workspace workspace);
}
