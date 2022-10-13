using Notescrib.Api.Core.Entities;
using MongoDB.Driver;
using Notescrib.Api.Application.Workspaces;
using Notescrib.Api.Infrastructure.MongoDb.Providers;

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

    public async Task<ICollection<Workspace>> GetUserWorkspaces(string ownerId)
    {
        var result = await _workspaces.Collection.FindAsync(x => x.OwnerId == ownerId);
        return await result.ToListAsync();
    }
}
