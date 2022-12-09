using MongoDB.Driver;

namespace Notescrib.Notes.Features.Workspaces.Repositories;

public class WorkspaceMongoRepository : IWorkspaceRepository
{
    private readonly IMongoCollection<Workspace> _collection;

    public WorkspaceMongoRepository(IMongoCollection<Workspace> collection)
    {
        _collection = collection;
    }

    public Task<Workspace?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        => _collection.Find(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken)!;

    public Task<Workspace?> GetByOwnerIdAsync(string ownerId, CancellationToken cancellationToken = default)
        => _collection.Find(x => x.OwnerId == ownerId).FirstOrDefaultAsync(cancellationToken)!;

    public Task AddAsync(Workspace workspace, CancellationToken cancellationToken = default)
        => _collection.InsertOneAsync(workspace, cancellationToken: cancellationToken);

    public Task UpdateAsync(Workspace workspace, CancellationToken cancellationToken = default)
        => _collection.ReplaceOneAsync(x => x.Id == workspace.Id,
            workspace,
            cancellationToken: cancellationToken);

    public Task DeleteAsync(string workspaceId, CancellationToken cancellationToken = default)
        => _collection.DeleteManyAsync(x => x.Id == workspaceId, cancellationToken);
}
