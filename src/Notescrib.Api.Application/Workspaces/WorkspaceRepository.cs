using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Application.Common;

namespace Notescrib.Api.Application.Workspaces;

internal class WorkspaceRepository : IWorkspaceRepository
{
    private readonly IPersistenceProvider<Workspace> _workspaces;

    public WorkspaceRepository(IPersistenceProvider<Workspace> workspaces)
    {
        _workspaces = workspaces;
    }

    public async Task<string> AddWorkspaceAsync(Workspace workspace)
        => await _workspaces.AddAsync(workspace);

    public async Task<Workspace?> GetWorkspaceByIdAsync(string workspaceId)
        => await _workspaces.FindByIdAsync(workspaceId);

    public async Task DeleteWorkspaceAsync(string workspaceId)
        => await _workspaces.DeleteAsync(workspaceId);

    public async Task UpdateWorkspaceAsync(Workspace workspace)
        => await _workspaces.UpdateAsync(workspace);

    public async Task<IPagedList<Workspace>> GetUserWorkspacesAsync(string ownerId, IPaging paging, ISorting? sorting = null)
        => await _workspaces.FindPagedAsync(x => x.OwnerId == ownerId, paging, sorting);
}
