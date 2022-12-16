using MongoDB.Bson;
using MongoDB.Driver;
using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes.Features.Notes.Repositories;

public class MongoNoteContentRepository : INoteContentRepository
{
    private readonly MongoDbContext _context;

    public MongoNoteContentRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<NoteContent?> GetByNoteIdAsync(string noteId, CancellationToken cancellationToken = default)
    {
        var noteFilter = MongoDbHelpers.GetNoteFilter(noteId);
        var projection = Builders<FolderData>.Projection
            .Include(x => x.Notes)
            .ElemMatch(x => x.Notes, x => x.Id == noteId);

        var folderPipeline = new EmptyPipelineDefinition<FolderData>()
            .Match(noteFilter)
            .Project(projection)
            .As<FolderData, BsonDocument, FolderData>();

        var result = await _context.NoteContents.Aggregate()
            .Match(x => x.NoteId == noteId)
            .Lookup(_context.Folders, new BsonDocument("", ""), folderPipeline,
                (NoteContentFolderLookup x) => x.Folders)
            .FirstOrDefaultAsync(cancellationToken);

        var note = result?.Folders.FirstOrDefault()?.Notes.FirstOrDefault();
        
        if (result == null || note == null)
        {
            return null;
        }

        return new() { NoteId = result.NoteId, Value = result.Value, Note = note };
    }

    public Task CreateAsync(string noteId, CancellationToken cancellationToken = default)
        => _context.NoteContents.InsertOneAsync(new() { NoteId = noteId, Value = string.Empty }, cancellationToken: cancellationToken);

    public Task UpdateAsync(NoteContentData content, CancellationToken cancellationToken = default)
    {
        var update = Builders<NoteContentData>.Update
            .Set(x => x.Value, content.Value);

        return _context.NoteContents.UpdateOneAsync(x => x.NoteId == content.NoteId, update, cancellationToken: cancellationToken);
    }

    public Task DeleteAsync(string noteId, CancellationToken cancellationToken = default)
        => _context.NoteContents.DeleteOneAsync(x => x.NoteId == noteId, cancellationToken);

    public Task DeleteManyAsync(IEnumerable<string> noteIds, CancellationToken cancellationToken = default)
        => _context.NoteContents.DeleteManyAsync(x => noteIds.Contains(x.NoteId), cancellationToken);
}
