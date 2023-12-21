using System.Linq.Expressions;
using MongoDB.Driver;
using Notescrib.Core.Services;
using Notescrib.Data.MongoDb;
using Notescrib.Extensions;

namespace Notescrib.Features.Folders.Repositories;

public class MongoFolderRepository : IFolderRepository
{
    private readonly IMongoDbProvider _provider;
    private readonly SessionAccessor _sessionAccessor;
    private readonly IUserContextProvider _userContextProvider;

    private static readonly AggregateUnwindOptions<Folder> UnwindOptions = new() { PreserveNullAndEmptyArrays = true };

    public MongoFolderRepository(IMongoDbProvider provider, SessionAccessor sessionAccessor,
        IUserContextProvider userContextProvider)
    {
        _provider = provider;
        _sessionAccessor = sessionAccessor;
        _userContextProvider = userContextProvider;
    }

    public Task CreateAsync(Folder folder, CancellationToken cancellationToken = default)
    {
        var folderData = (FolderData)folder;
        return _provider.Folders.SessionInsertOneAsync(_sessionAccessor.Session, folderData,
            cancellationToken: cancellationToken);
    }

    public Task<Folder?> GetByIdAsync(string id, FolderIncludeOptions? include = null,
        CancellationToken cancellationToken = default)
        => GetWithInclude(x => x.Id == id, include, cancellationToken);

    public Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        => _provider.Folders.SessionDeleteManyAsync(_sessionAccessor.Session, x => ids.Contains(x.Id),
            cancellationToken: cancellationToken);

    public Task DeleteAllAsync(CancellationToken cancellationToken = default)
        => _provider.Folders.SessionDeleteManyAsync(_sessionAccessor.Session,
            x => x.OwnerId == _userContextProvider.UserId,
            cancellationToken: cancellationToken);

    public Task UpdateAsync(Folder folder, CancellationToken cancellationToken = default)
    {
        var update = Builders<FolderData>.Update
            .Set(x => x.Name, folder.Name)
            .Set(x => x.Updated, folder.Updated);

        return _provider.Folders.SessionUpdateOneAsync(_sessionAccessor.Session, x => x.Id == folder.Id, update,
            cancellationToken: cancellationToken);
    }

    public Task<Folder?> GetRootAsync(FolderIncludeOptions? include = null,
        CancellationToken cancellationToken = default)
    {
        include ??= new();

        var aggregate = _provider.Folders
            .SessionAggregate(_sessionAccessor.Session)
            .Match(x => x.OwnerId == _userContextProvider.UserId
                        && x.ParentId == null)
            .As<Folder>();

        aggregate = Include(aggregate, include);

        return aggregate.As<Folder>().FirstOrDefaultAsync(cancellationToken)!;
    }

    private Task<Folder?> GetWithInclude(Expression<Func<FolderData, bool>> filter, FolderIncludeOptions? include,
        CancellationToken cancellationToken)
        => GetWithInclude(new ExpressionFilterDefinition<FolderData>(filter), include, cancellationToken);

    private Task<Folder?> GetWithInclude(FilterDefinition<FolderData> filter, FolderIncludeOptions? include,
        CancellationToken cancellationToken)
    {
        include ??= new();

        var aggregate = _provider.Folders
            .SessionAggregate(_sessionAccessor.Session)
            .Match(filter)
            .As<Folder>();

        aggregate = Include(aggregate, include);

        return aggregate.FirstOrDefaultAsync(cancellationToken)!;
    }

    private IAggregateFluent<Folder> Include(IAggregateFluent<Folder> query,
        FolderIncludeOptions options)
    {
        if (options.Workspace)
        {
            query = query
                .Lookup(
                    _provider.Workspaces,
                    x => x.WorkspaceId,
                    x => x.Id,
                    (Folder x) => x.Workspace)
                .Unwind(x => x.Workspace, UnwindOptions);
        }

        if (options.ImmediateChildren)
        {
            query = query
                .Lookup(
                    _provider.Folders,
                    x => x.Id,
                    x => x.ParentId,
                    (Folder x) => x.ImmediateChildren);
        }

        if (options.Children)
        {
            query = query.Lookup(
                _provider.Folders,
                x => x.Id,
                x => x.AncestorIds,
                (Folder x) => x.Children);
        }

        if (options.Parent)
        {
            query = query
                .Lookup(
                    _provider.Folders,
                    x => x.Id,
                    x => x.ParentId,
                    (Folder x) => x.Parent)
                .Unwind(x => x.Parent, UnwindOptions);
        }

        return query;
    }
}
