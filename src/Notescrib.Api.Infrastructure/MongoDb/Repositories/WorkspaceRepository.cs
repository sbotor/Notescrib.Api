using Notescrib.Api.Core.Entities;
using Notescrib.Api.Application.Workspaces;
using Notescrib.Api.Infrastructure.MongoDb.Providers;
using Notescrib.Api.Core.Models;
using Notescrib.Api.Core.Contracts;

namespace Notescrib.Api.Infrastructure.MongoDb.Repositories;

internal class WorkspaceRepository : IWorkspaceRepository
{
    private readonly IMongoPersistenceProvider<Workspace> _workspaces;

    public WorkspaceRepository(IMongoPersistenceProvider<Workspace> workspaces)
    {
        _workspaces = workspaces;
    }

    public async Task<Workspace> AddWorkspaceAsync(Workspace workspace)
        => await _workspaces.AddAsync(workspace);

    public async Task<Workspace?> GetWorkspaceByIdAsync(string workspaceId)
        => await _workspaces.FindByIdAsync(workspaceId);

    public async Task<bool> DeleteWorkspaceAsync(string workspaceId)
        => await _workspaces.DeleteAsync(workspaceId);

    public async Task UpdateWorkspaceAsync(Workspace workspace)
        => await _workspaces.UpdateAsync(workspace);

    public async Task<PagedList<Workspace>> GetUserWorkspacesAsync(string ownerId, IPaging paging, ISorting? sorting = null)
        => await _workspaces.FindPagedAsync(x => x.OwnerId == ownerId, paging, sorting);
}
