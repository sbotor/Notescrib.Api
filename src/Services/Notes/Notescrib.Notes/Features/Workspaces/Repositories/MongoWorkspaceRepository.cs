using MongoDB.Driver;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes.Features.Workspaces.Repositories;

public class MongoWorkspaceRepository : IWorkspaceRepository
{
    private readonly IMongoDbProvider _provider;
    private readonly SessionAccessor _sessionAccessor;

    public MongoWorkspaceRepository(IMongoDbProvider provider, SessionAccessor sessionAccessor)
    {
        _provider = provider;
        _sessionAccessor = sessionAccessor;
    }

    public async Task<Workspace?> GetByOwnerIdAsync(string ownerId, CancellationToken cancellationToken = default)
        => await _provider.Workspaces.SessionFind(_sessionAccessor.Session, x => x.OwnerId == ownerId)
            .FirstOrDefaultAsync(cancellationToken);

    public Task AddAsync(Workspace workspace, CancellationToken cancellationToken = default)
        => _provider.Workspaces.SessionInsertOneAsync(_sessionAccessor.Session, workspace,
            cancellationToken: cancellationToken);

    public Task DeleteAsync(string workspaceId, CancellationToken cancellationToken = default)
        => _provider.Workspaces.SessionDeleteManyAsync(_sessionAccessor.Session, x => x.Id == workspaceId,
            cancellationToken: cancellationToken);
}
