using MongoDB.Driver;
using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Workspaces;

namespace Notescrib.Notes.Features.Folders.Repositories;

public class FolderMongoRepository : IFolderRepository
{
    private static readonly AggregateUnwindOptions<Folder> UnwindOptions = new()
    {
        PreserveNullAndEmptyArrays = true
    };

    private readonly IMongoCollection<FolderBase> _collection;
    private readonly IMongoCollection<Workspace> _workspaceCollection;
    private readonly IMongoCollection<Note> _noteCollection;

    public FolderMongoRepository(IMongoCollection<FolderBase> collection, IMongoCollection<Workspace> workspaceCollection, IMongoCollection<Note> noteCollection)
    {
        _collection = collection;
        _workspaceCollection = workspaceCollection;
        _noteCollection = noteCollection;
    }

    public Task AddAsync(FolderBase folder, CancellationToken cancellationToken = default)
        => _collection.InsertOneAsync(folder, cancellationToken: cancellationToken);

    public Task<Folder?> GetByIdAsync(string id, FolderIncludeOptions? include = null,
        CancellationToken cancellationToken = default)
    {
        include ??= new();

        var aggregate = _collection.Aggregate()
            .Match(x => x.Id == id)
            .As<Folder>();

        aggregate = Include(aggregate, include);

        return aggregate.As<Folder>().FirstOrDefaultAsync(cancellationToken)!;
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        => _collection.DeleteOneAsync(x => x.Id == id, cancellationToken);
    
    public Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        => _collection.DeleteOneAsync(x => ids.Contains(x.Id), cancellationToken);
    
    public Task UpdateAsync(FolderBase folder, CancellationToken cancellationToken = default)
        => _collection.ReplaceOneAsync(x => x.Id == folder.Id, folder, cancellationToken: cancellationToken);

    public Task<Folder?> GetRootAsync(string ownerId, FolderIncludeOptions? include = null, CancellationToken cancellationToken = default)
    {
        include ??= new();

        var aggregate = _collection.Aggregate()
            .Match(x => x.OwnerId == ownerId && x.ParentId == null)
            .As<Folder>();

        aggregate = Include(aggregate, include);

        return aggregate.As<Folder>().FirstOrDefaultAsync(cancellationToken)!;
    }

    private IAggregateFluent<Folder> Include(IAggregateFluent<Folder> aggregate,
        FolderIncludeOptions options)
    {
        if (options.Workspace)
        {
            aggregate = aggregate
                .Lookup(
                    _workspaceCollection,
                    x => x.WorkspaceId,
                    x => x.Id,
                    (Folder x) => x.Workspace)
                .Unwind(x => x.Workspace, UnwindOptions);
        }
        
        if (options.ImmediateChildren)
        {
            aggregate = aggregate
                .Lookup(
                    _collection,
                    x => x.Id, 
                    x => x.ParentId, 
                    (Folder x) => x.ImmediateChildren);
        }

        if (options.Children)
        {
            aggregate = aggregate.Lookup(
                _collection,
                x => x.Id,
                x => x.AncestorIds,
                (Folder x) => x.Children);
        }

        if (options.ImmediateNotes)
        {
            aggregate = aggregate.Lookup(
                _noteCollection,
                x => x.Id,
                x => x.FolderId,
                (Folder x) => x.ImmediateNotes);
        }
        
        if (options.Parent)
        {
            aggregate = aggregate
                .Lookup(
                    _collection,
                    x => x.Id,
                    x => x.ParentId,
                    (Folder x) => x.Parent)
                .Unwind(x => x.Parent, UnwindOptions);
        }

        return aggregate;
    }
}
