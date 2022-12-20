using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Notes.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Models.Enums;
using Notescrib.Notes.Utils;
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

    public async Task<IReadOnlyCollection<NoteBase>> GetManyAsync(IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
        => await _context.Notes
            .Find(x => ids.Contains(x.Id))
            .Project(Builders<NoteData>.Projection.Exclude(x => x.Content))
            .As<NoteBase>()
            .ToListAsync(cancellationToken);

    public Task CreateAsync(NoteData note, CancellationToken cancellationToken = default)
        => _context.Notes.InsertOneAsync(note, cancellationToken: cancellationToken);

    public Task UpdateAsync(NoteData note, CancellationToken cancellationToken = default)
    {
        var update = Builders<NoteData>.Update
            .Set(x => x.Name, note.Name)
            .Set(x => x.Tags, note.Tags)
            .Set(x => x.SharingInfo, note.SharingInfo)
            .Set(x => x.RelatedIds, note.RelatedIds)
            .Set(x => x.Updated, note.Updated);

        return _context.Notes.UpdateOneAsync(x => x.Id == note.Id, update, cancellationToken: cancellationToken);
    }

    public Task UpdateContentAsync(NoteData note, CancellationToken cancellationToken = default)
    {
        var update = Builders<NoteData>.Update
            .Set(x => x.Content, note.Content)
            .Set(x => x.Updated, note.Updated);

        return _context.Notes.UpdateOneAsync(x => x.Id == note.Id, update, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(NoteBase note, CancellationToken cancellationToken = default)
    {
        var update = Builders<NoteData>.Update.Pull(x => x.RelatedIds, note.Id);

        await _context.Notes.UpdateManyAsync(x => x.OwnerId == note.Id, update, cancellationToken: cancellationToken);

        await _context.Notes.DeleteOneAsync(x => x.Id == note.Id, cancellationToken);
    }

    public async Task DeleteFromFoldersAsync(string ownerId, IEnumerable<string> folderIds, CancellationToken cancellationToken = default)
    {
        var enumeratedFolderIds = folderIds.ToArray();
        
        var noteIds = await _context.Notes.Aggregate()
            .Match(x => enumeratedFolderIds.Contains(x.FolderId))
            .Project(Builders<NoteData>.Projection.Include(x => x.Id))
            .As<string>()
            .ToListAsync(cancellationToken);
        
        var update = Builders<NoteData>.Update
            .PullAll(x => x.RelatedIds, noteIds);

        await _context.Notes.UpdateManyAsync(x => x.OwnerId == ownerId, update, cancellationToken: cancellationToken);
        
        await _context.Notes.DeleteManyAsync(x => noteIds.Contains(x.Id), cancellationToken);
    }

    public Task<PagedList<NoteBase>> SearchAsync(string ownerId, string? textFilter, bool ownOnly,
        PagingSortingInfo<NotesSorting> info,
        CancellationToken cancellationToken = default)
    {
        var filters = new List<ExpressionFilterDefinition<NoteData>>();

        if (ownOnly)
        {
            filters.Add(new(x => x.OwnerId == ownerId));
        }
        else
        {
            filters.Add(new(x => x.OwnerId == ownerId || x.SharingInfo.Visibility == VisibilityLevel.Public));
        }

        if (!string.IsNullOrEmpty(textFilter))
        {
            filters.Add(new(x => x.Name.Contains(textFilter)));
        }

        ProjectionDefinition<NoteData, NoteBase> projection = Builders<NoteData>.Projection
            .Exclude(x => x.Content);

        return _context.Notes.FindPagedAsync(filters, info, projection, cancellationToken);
    }

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

        if (include.Related)
        {
            var let = new BsonDocument {{ "noteId", $"${nameof(NoteData.Id)}" }};

            ProjectionDefinition<NoteData, NoteBase> projection = Builders<NoteData>.Projection.Exclude(x => x.Content);

            var pipeline = new EmptyPipelineDefinition<NoteData>()
                .Match(new BsonDocument {{ nameof(NoteData.Id), $"$${nameof(NoteData.Id)}" }})
                .Project(projection);

            query = query.Lookup(
                _context.Notes,
                let,
                pipeline,
                (Note x) => x.Related);
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
