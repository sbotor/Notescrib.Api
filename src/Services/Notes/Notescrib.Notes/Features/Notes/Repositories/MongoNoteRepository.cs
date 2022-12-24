using System.Linq.Expressions;
using MongoDB.Driver;
using Notescrib.Core.Services;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Notes.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Models.Enums;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.MongoDb;
using Notescrib.Notes.Utils.MongoDb.Models;

namespace Notescrib.Notes.Features.Notes.Repositories;

public class MongoNoteRepository : INoteRepository
{
    private readonly IMongoDbProvider _provider;
    private readonly SessionAccessor _sessionAccessor;
    private readonly IUserContextProvider _userContextProvider;

    public MongoNoteRepository(IMongoDbProvider provider, SessionAccessor sessionAccessor,
        IUserContextProvider userContextProvider)
    {
        _provider = provider;
        _sessionAccessor = sessionAccessor;
        _userContextProvider = userContextProvider;
    }

    public Task<Note?> GetByIdAsync(string id, NoteIncludeOptions? include = null,
        CancellationToken cancellationToken = default)
        => GetWithInclude(x => x.Id == id, include).FirstOrDefaultAsync(cancellationToken)!;

    public async Task<IReadOnlyCollection<Note>> GetByFolderIdAsync(string folderId, NoteIncludeOptions? include = null,
        CancellationToken cancellationToken = default)
        => await GetWithInclude(x => x.FolderId == folderId, include).ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Note>> GetManyAsync(IEnumerable<string> ids,
        CancellationToken cancellationToken = default)
        => await _provider.Notes
            .SessionFind(_sessionAccessor.Session, x => ids.Contains(x.Id))
            .Project(Builders<NoteData>.Projection.Exclude(x => x.Content))
            .As<Note>()
            .ToListAsync(cancellationToken);

    public Task CreateAsync(Note note, CancellationToken cancellationToken = default)
    {
        var noteData = (NoteData)note;
        return _provider.Notes.SessionInsertOneAsync(_sessionAccessor.Session, noteData, cancellationToken);
    }

    public Task UpdateAsync(Note note, CancellationToken cancellationToken = default)
    {
        var update = Builders<NoteData>.Update
            .Set(x => x.Name, note.Name)
            .Set(x => x.Tags, note.Tags)
            .Set(x => x.SharingInfo, note.SharingInfo)
            .Set(x => x.Updated, note.Updated);

        return _provider.Notes.SessionUpdateOneAsync(_sessionAccessor.Session, x => x.Id == note.Id, update,
            cancellationToken);
    }
    
    public Task UpdateRelatedAsync(Note note, CancellationToken cancellationToken = default)
    {
        var update = Builders<NoteData>.Update
            .Set(x => x.RelatedIds, note.RelatedIds);

        return _provider.Notes.SessionUpdateOneAsync(_sessionAccessor.Session, x => x.Id == note.Id, update,
            cancellationToken);
    }

    public Task UpdateContentAsync(Note note, CancellationToken cancellationToken = default)
    {
        var update = Builders<NoteData>.Update
            .Set(x => x.Content, note.Content)
            .Set(x => x.Updated, note.Updated);

        return _provider.Notes.SessionUpdateOneAsync(_sessionAccessor.Session, x => x.Id == note.Id, update,
            cancellationToken);
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        => _provider.Notes.SessionDeleteOneAsync(_sessionAccessor.Session, x => x.Id == id, cancellationToken);

    public Task DeleteAllAsync(CancellationToken cancellationToken = default)
        => _provider.Notes.SessionDeleteManyAsync(
            _sessionAccessor.Session,
            x => x.OwnerId == _userContextProvider.UserId,
            cancellationToken);

    public Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        => _provider.Notes.SessionDeleteManyAsync(_sessionAccessor.Session, x => ids.Contains(x.Id),
            cancellationToken);

    public async Task<IReadOnlyCollection<string>> GetIdsFromFoldersAsync(IEnumerable<string> folderIds,
        CancellationToken cancellationToken = default)
    {
        ProjectionDefinition<NoteData, IdProjection> project = Builders<NoteData>.Projection.Include(x => x.Id);

        var result = await _provider.Notes
            .SessionAggregate(_sessionAccessor.Session)
            .Match(x => folderIds.Contains(x.FolderId))
            .Project(project)
            .ToListAsync(cancellationToken);

        return result.Select(x => x.Id).ToArray();
    }

    public Task DeleteFromRelatedAsync(IEnumerable<string> noteIds, CancellationToken cancellationToken = default)
    {
        var update = Builders<NoteData>.Update
            .PullAll(x => x.RelatedIds, noteIds);

        return _provider.Notes.SessionUpdateManyAsync(
            _sessionAccessor.Session,
            x => x.OwnerId == _userContextProvider.UserId,
            update,
            cancellationToken: cancellationToken);
    }

    public Task DeleteFromRelatedAsync(string id, CancellationToken cancellationToken = default)
    {
        var update = Builders<NoteData>.Update
            .Pull(x => x.RelatedIds, id);

        return _provider.Notes.SessionUpdateManyAsync(
            _sessionAccessor.Session,
            x => x.OwnerId == _userContextProvider.UserId,
            update,
            cancellationToken: cancellationToken);
    }

    public Task<PagedList<Note>> SearchAsync(string? ownerId, string? textFilter, bool ownOnly,
        PagingSortingInfo<NotesSorting> info,
        CancellationToken cancellationToken = default)
    {
        var filters = new List<FilterDefinition<NoteData>>();

        if (string.IsNullOrEmpty(ownerId))
        {
            filters.Add(
                new ExpressionFilterDefinition<NoteData>(x => x.SharingInfo.Visibility == VisibilityLevel.Public));
        }
        else if (ownOnly)
        {
            filters.Add(new ExpressionFilterDefinition<NoteData>(x => x.OwnerId == ownerId));
        }
        else
        {
            filters.Add(new ExpressionFilterDefinition<NoteData>(x => x.OwnerId == ownerId
                                                                      || x.SharingInfo.Visibility ==
                                                                      VisibilityLevel.Public));
        }

        if (!string.IsNullOrEmpty(textFilter))
        {
            filters.Add(new ExpressionFilterDefinition<NoteData>(x => x.Name.Contains(textFilter)
                                                                      || x.Tags.Any(t => t.Contains(textFilter))));
        }

        ProjectionDefinition<NoteData, Note> projection = Builders<NoteData>.Projection
            .Exclude(x => x.Content);

        return _provider.Notes.FindPagedAsync(_sessionAccessor.Session, filters, info, projection, cancellationToken);
    }

    private IAggregateFluent<Note> Include(IAggregateFluent<Note> query, NoteIncludeOptions include)
    {
        if (include.Folder)
        {
            query = query
                .Lookup(
                    _provider.Folders,
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
            ProjectionDefinition<Note, Note> projection = Builders<Note>.Projection
                .Exclude($"{nameof(Note.Related)}.{nameof(Note.Content)}");

            query = query.Lookup(
                _provider.Notes,
                x => x.RelatedIds,
                x => x.Id,
                (Note x) => x.Related)
                .Project(projection);
        }

        return query;
    }

    private IAggregateFluent<Note> GetWithInclude(Expression<Func<NoteData, bool>> filter, NoteIncludeOptions? include)
    {
        include ??= new();

        var query = _provider.Notes
            .SessionAggregate(_sessionAccessor.Session)
            .Match(filter)
            .As<Note>();

        return Include(query, include);
    }
}
