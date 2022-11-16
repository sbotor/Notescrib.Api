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

    public Task<PagedList<Note>> GetNotesAsync(
        string? workspaceId,
        string? folder,
        IPermissionGuard permissionGuard,
        PagingSortingInfo<NotesSorting> info,
        CancellationToken cancellationToken = default)
    {
        var filters = new[]
        {
            x => (workspaceId == null || x.WorkspaceId == workspaceId)
                 && (folder == null || x.Folder == folder),
            permissionGuard.ExpressionCanView<Note>()
        };

        return _collection.FindPagedAsync(
            filters,
            info,
            cancellationToken);
    }

    public Task AddNote(Note note, CancellationToken cancellationToken = default)
        => _collection.InsertOneAsync(note, cancellationToken: cancellationToken);

    public async Task<bool> ExistsAsync(
        string workspaceId,
        string folder,
        string name,
        CancellationToken cancellationToken = default)
    {
        var found = await _collection.Find(
                x => x.WorkspaceId == workspaceId
                        && x.Folder == folder && x.Name == name)
            .FirstOrDefaultAsync(cancellationToken);

        return found != null;
    }

    public Task<Note?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        => _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken)!;

    public Task UpdateNoteAsync(Note note, CancellationToken cancellationToken = default)
        => _collection.FindOneAndReplaceAsync(
            x => x.Id == note.Id,
            note,
            cancellationToken: cancellationToken);
}
