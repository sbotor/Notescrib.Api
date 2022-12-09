using MongoDB.Driver;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Notes.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Repositories;

public class NoteMongoRepository : INoteRepository
{
    private readonly IMongoCollection<Note> _collection;

    public NoteMongoRepository(IMongoCollection<Note> collection)
    {
        _collection = collection;
    }

    public Task<PagedList<Note>> GetAsync(
        string? folderId,
        IPermissionGuard permissionGuard,
        PagingSortingInfo<NotesSorting> info,
        CancellationToken cancellationToken = default)
    {
        var filters = new[]
        {
            x => folderId == null || x.FolderId == folderId,
            permissionGuard.ExpressionCanView<Note>()
        };

        return _collection.FindPagedAsync(
            filters,
            info,
            cancellationToken);
    }
    
    public Task<IReadOnlyCollection<Note>> GetAsync(
        string? folderId,
        IPermissionGuard permissionGuard,
        CancellationToken cancellationToken = default)
    {
        var filters = new[]
        {
            x => folderId == null || x.FolderId == folderId,
            permissionGuard.ExpressionCanView<Note>()
        };

        return _collection.FindAsync(filters, cancellationToken);
    }

    public Task AddNote(Note note, CancellationToken cancellationToken = default)
        => _collection.InsertOneAsync(note, cancellationToken: cancellationToken);

    public Task<Note?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        => _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken)!;

    public Task UpdateAsync(Note note, CancellationToken cancellationToken = default)
        => _collection.FindOneAndReplaceAsync(
            x => x.Id == note.Id,
            note,
            cancellationToken: cancellationToken);

    public Task DeleteFromFoldersAsync(IEnumerable<string> folderIds,
        CancellationToken cancellationToken = default)
        => _collection.DeleteManyAsync(
            x => folderIds.Contains(x.FolderId),
            cancellationToken: cancellationToken);
}
