using MongoDB.Driver;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Workspaces.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Workspaces.Repositories;

public class WorkspaceMongoRepository : IWorkspaceRepository
{
    private readonly IMongoCollection<Workspace> _collection;

    public WorkspaceMongoRepository(IMongoCollection<Workspace> collection)
    {
        _collection = collection;
    }
    
    public Task<Workspace?> GetWorkspaceByIdAsync(string id, CancellationToken cancellationToken = default)
        => _collection.Find(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken)!;
    
    public Task<PagedList<Workspace>> GetWorkspacesByOwnerIdAsync(
        string ownerId,
        PagingSortingInfo<WorkspacesSorting> info,
        CancellationToken cancellationToken = default)
        => _collection.FindPagedAsync(
            x => x.OwnerId == ownerId,
            info,
            cancellationToken);

    public Task AddWorkspaceAsync(Workspace workspace, CancellationToken cancellationToken = default)
        => _collection.InsertOneAsync(workspace, cancellationToken: cancellationToken);
    
    public Task UpdateWorkspaceAsync(Workspace workspace, CancellationToken cancellationToken = default)
        => _collection.ReplaceOneAsync(x => x.Id == workspace.Id,
            workspace,
            cancellationToken: cancellationToken);

    public async Task<bool> ExistsAsync(string ownerId, string name, CancellationToken cancellationToken = default)
    {
        var found = await _collection
            .Find(x => x.OwnerId == ownerId && x.Name == name)
            .FirstOrDefaultAsync(cancellationToken);

        return found != null;
    }

    public Task DeleteAsync(string workspaceId, CancellationToken cancellationToken = default)
        => _collection.DeleteManyAsync(x => x.Id == workspaceId, cancellationToken);
}
