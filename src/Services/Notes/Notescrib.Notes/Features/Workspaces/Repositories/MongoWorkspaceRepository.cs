using MongoDB.Driver;
using Notescrib.Core.Services;
using Notescrib.Notes.Data.MongoDb;
using Notescrib.Notes.Extensions;

namespace Notescrib.Notes.Features.Workspaces.Repositories;

public class MongoWorkspaceRepository : IWorkspaceRepository
{
    private readonly IMongoDbProvider _provider;
    private readonly SessionAccessor _sessionAccessor;
    private readonly IUserContextProvider _userContextProvider;

    public MongoWorkspaceRepository(IMongoDbProvider provider, SessionAccessor sessionAccessor,
        IUserContextProvider userContextProvider)
    {
        _provider = provider;
        _sessionAccessor = sessionAccessor;
        _userContextProvider = userContextProvider;
    }

    public async Task<Workspace?> GetByOwnerIdAsync(CancellationToken cancellationToken = default)
        => await _provider.Workspaces
            .SessionFind(_sessionAccessor.Session, x => x.OwnerId == _userContextProvider.UserId)
            .FirstOrDefaultAsync(cancellationToken);

    public Task AddAsync(Workspace workspace, CancellationToken cancellationToken = default)
        => _provider.Workspaces.SessionInsertOneAsync(_sessionAccessor.Session, workspace,
            cancellationToken: cancellationToken);

    public Task DeleteAsync(string workspaceId, CancellationToken cancellationToken = default)
        => _provider.Workspaces.SessionDeleteManyAsync(_sessionAccessor.Session, x => x.Id == workspaceId,
            cancellationToken: cancellationToken);
}
