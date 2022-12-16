using MongoDB.Driver;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes.Features.Workspaces.Repositories;

public class MongoWorkspaceRepository : IWorkspaceRepository
{
    private readonly MongoDbContext _context;

    public MongoWorkspaceRepository(MongoDbContext context)
    {
        _context = context;
    }

    public Task<Workspace?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        => _context.Workspaces.Find(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken)!;

    public Task<Workspace?> GetByOwnerIdAsync(string ownerId, CancellationToken cancellationToken = default)
        => _context.Workspaces.Find(x => x.OwnerId == ownerId).FirstOrDefaultAsync(cancellationToken)!;

    public Task AddAsync(Workspace workspace, CancellationToken cancellationToken = default)
        => _context.Workspaces.InsertOneAsync(workspace, cancellationToken: cancellationToken);

    public Task UpdateAsync(Workspace workspace, CancellationToken cancellationToken = default)
        => _context.Workspaces.ReplaceOneAsync(x => x.Id == workspace.Id,
            workspace,
            cancellationToken: cancellationToken);

    public Task DeleteAsync(string workspaceId, CancellationToken cancellationToken = default)
        => _context.Workspaces.DeleteManyAsync(x => x.Id == workspaceId, cancellationToken);
}
