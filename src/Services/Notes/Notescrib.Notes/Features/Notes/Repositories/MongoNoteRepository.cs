using System.Linq.Expressions;
using MongoDB.Driver;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes.Features.Notes.Repositories;

public class MongoNoteRepository : INoteRepository
{
    private readonly MongoDbContext _context;

    public MongoNoteRepository(MongoDbContext context)
    {
        _context = context;
    }

    public Task<Note?> GetByIdAsync(string id, NoteIncludeOptions? include = null,
        CancellationToken cancellationToken = default)
        => GetWithInclude(x => x.Id == id, include).FirstOrDefaultAsync(cancellationToken)!;

    public async Task<IReadOnlyCollection<Note>> GetByFolderIdAsync(string folderId, NoteIncludeOptions? include = null,
        CancellationToken cancellationToken = default)
        => await GetWithInclude(x => x.FolderId == folderId, include).ToListAsync(cancellationToken);

    public Task CreateAsync(NoteData note, CancellationToken cancellationToken = default)
        => _context.Notes.InsertOneAsync(note, cancellationToken: cancellationToken);

    public Task UpdateAsync(NoteData note, CancellationToken cancellationToken = default)
    {
        var update = Builders<NoteData>.Update
            .Set(x => x.Name, note.Name)
            .Set(x => x.Tags, note.Tags)
            .Set(x => x.SharingInfo, note.SharingInfo)
            .Set(x => x.Updated, note.Updated);

        return _context.Notes.UpdateOneAsync(x => x.Id == note.Id, update, cancellationToken: cancellationToken);
    }

    public Task UpdateContentAsync(string id, string content, CancellationToken cancellationToken = default)
    {
        var update = Builders<NoteData>.Update
            .Set(x => x.Content, content);

        return _context.Notes.UpdateOneAsync(x => x.Id == id, update, cancellationToken: cancellationToken);
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        => _context.Notes.DeleteOneAsync(x => x.Id == id, cancellationToken);

    public Task DeleteFromFoldersAsync(IEnumerable<string> folderIds, CancellationToken cancellationToken = default)
        => _context.Notes.DeleteManyAsync(x => folderIds.Contains(x.FolderId), cancellationToken);

    private IAggregateFluent<Note> Include(IAggregateFluent<Note> query, NoteIncludeOptions include)
    {
        if (include.Folder)
        {
            query = query
                .Lookup(
                    _context.Folders,
                    x => x.FolderId,
                    x => x.Id,
                    (Note x) => x.Folder);
        }

        if (!include.Content)
        {
            ProjectionDefinition<Note, Note> projection = Builders<Note>.Projection
                .Exclude(x => x.Content);

            query = query
                .Project(projection);
        }

        return query;
    }

    private IAggregateFluent<Note> GetWithInclude(Expression<Func<NoteData, bool>> filter, NoteIncludeOptions? include)
    {
        include ??= new();

        var query = _context.Notes.Aggregate()
            .Match(filter)
            .As<Note>();

        return Include(query, include);
    }
}
