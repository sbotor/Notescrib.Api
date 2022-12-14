using System.Linq.Expressions;
using MongoDB.Driver;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Notes.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes.Features.Notes.Repositories;

public class NoteMongoRepository : INoteRepository
{
    private readonly MongoDbContext _context;

    public NoteMongoRepository(MongoDbContext context)
    {
        _context = context;
    }

    public Task CreateNote(NoteBase note, CancellationToken cancellationToken = default)
        => _context.Notes.InsertOneAsync(note, cancellationToken: cancellationToken);

    public Task<Note?> GetByIdAsync(string id, NoteIncludeOptions? include = null, CancellationToken cancellationToken = default)
    {
        include ??= new();
        
        var aggregate = _context.Notes.Aggregate()
            .Match(x => x.Id == id)
            .As<Note>();

        aggregate = Include(aggregate, include);

        return aggregate.FirstOrDefaultAsync(cancellationToken)!;
    }

    public Task UpdateAsync(NoteBase note, CancellationToken cancellationToken = default)
    {
        var update = Builders<NoteBase>.Update
            .Set(x => x.Name, note.Name)
            .Set(x => x.Tags, note.Tags)
            .Set(x => x.SharingInfo, note.SharingInfo)
            .Set(x => x.Updated, note.Updated);

        return _context.Notes.UpdateOneAsync(x => x.Id == note.Id, update, cancellationToken: cancellationToken);
    }
    
    public Task UpdateContentAsync(NoteBase note, CancellationToken cancellationToken = default)
    {
        var update = Builders<NoteBase>.Update
            .Set(x => x.Content, note.Content);

        return _context.Notes.UpdateOneAsync(x => x.Id == note.Id, update, cancellationToken: cancellationToken);
    }

    public async Task<int> DeleteFromFoldersAsync(IEnumerable<string> folderIds,
        CancellationToken cancellationToken = default)
    {
        var result = await _context.Notes.DeleteManyAsync(
            x => folderIds.Contains(x.FolderId),
            cancellationToken: cancellationToken);

        return (int)result.DeletedCount;
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        => _context.Notes.DeleteOneAsync(x => x.Id == id, cancellationToken: cancellationToken);

    private IAggregateFluent<Note> Include(IAggregateFluent<Note> aggregate, NoteIncludeOptions options)
    {
        if (!options.Content)
        {
            var projection = Builders<Note>.Projection
                .Exclude(x => x.Content);

            aggregate = aggregate
                .Project<Note>(projection);
        }
        
        if (options.Folder)
        {
            aggregate = aggregate
                .Lookup(
                    _context.Folders,
                    x => x.FolderId,
                    x => x.Id,
                    (Note x) => x.Folder);
        }

        if (options.Workspace)
        {
            aggregate = aggregate
                .Lookup(
                    _context.Workspaces,
                    x => x.WorkspaceId,
                    x => x.Id,
                    (Note x) => x.Workspace);
        }

        return aggregate;
    }
}
