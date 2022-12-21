using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Models;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes.Features.Folders.Repositories;

public class MongoFolderRepository : IFolderRepository
{
    private readonly MongoDbContext _context;

    private static readonly AggregateUnwindOptions<Folder> UnwindOptions = new()
    {
        PreserveNullAndEmptyArrays = true
    };

    public MongoFolderRepository(MongoDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(FolderData folder, CancellationToken cancellationToken = default)
        => _context.Folders.InsertOneAsync(folder, cancellationToken: cancellationToken);

    public Task<Folder?> GetByIdAsync(string id, FolderIncludeOptions? include = null,
        CancellationToken cancellationToken = default)
        => GetWithInclude(x => x.Id == id, include, cancellationToken);

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        => _context.Folders.DeleteOneAsync(x => x.Id == id, cancellationToken);
    
    public Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        => _context.Folders.DeleteManyAsync(x => ids.Contains(x.Id), cancellationToken);

    public Task DeleteAllAsync(string workspaceId, CancellationToken cancellationToken = default)
        => _context.Folders.DeleteManyAsync(x => x.WorkspaceId == workspaceId, cancellationToken);
    
    public Task UpdateAsync(FolderData folder, CancellationToken cancellationToken = default)
    {
        var update = Builders<FolderData>.Update
            .Set(x => x.Name, folder.Name)
            .Set(x => x.Updated, folder.Updated);

        return _context.Folders.UpdateOneAsync(x => x.Id == folder.Id, update, cancellationToken: cancellationToken);
    }

    public Task<Folder?> GetRootAsync(string ownerId, FolderIncludeOptions? include = null, CancellationToken cancellationToken = default)
    {
        include ??= new();

        var aggregate = _context.Folders.Aggregate()
            .Match(x => x.OwnerId == ownerId && x.ParentId == null)
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

        var aggregate = _context.Folders.Aggregate()
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
                    _context.Workspaces,
                    x => x.WorkspaceId,
                    x => x.Id,
                    (Folder x) => x.Workspace)
                .Unwind(x => x.Workspace, UnwindOptions);
        }
        
        if (options.ImmediateChildren)
        {
            query = query
                .Lookup(
                    _context.Folders,
                    x => x.Id, 
                    x => x.ParentId, 
                    (Folder x) => x.ImmediateChildren);
        }

        if (options.Children)
        {
            query = query.Lookup(
                _context.Folders,
                x => x.Id,
                x => x.AncestorIds,
                (Folder x) => x.Children);
        }

        if (options.Parent)
        {
            query = query
                .Lookup(
                    _context.Folders,
                    x => x.Id,
                    x => x.ParentId,
                    (Folder x) => x.Parent)
                .Unwind(x => x.Parent, UnwindOptions);
        }

        return query;
    }
}
