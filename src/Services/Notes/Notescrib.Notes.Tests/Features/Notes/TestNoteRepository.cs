using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Notes.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;
using Notescrib.Notes.Tests.Infrastructure;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Tests.Features.Notes;

public class TestNoteRepository : TestRepositoryBase<Note, NotesSorting>, INoteRepository
{
    public Task<PagedList<Note>> GetAsync(
        string? folderId,
        IPermissionGuard permissionGuard,
        PagingSortingInfo<NotesSorting> info,
        CancellationToken cancellationToken = default)
        => GetPaged(GetFilter(permissionGuard, folderId), info);

    public Task AddNote(Note note, CancellationToken cancellationToken = default)
        => Add(note, x => x.Id = Guid.NewGuid().ToString());

    public Task<Note?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        => GetSingleOrDefault(x => x.Id == id);

    public Task UpdateAsync(Note note, CancellationToken cancellationToken = default)
        => Update(note, x => x.Id == note.Id);

    public Task<IReadOnlyCollection<Note>> GetAsync(string? folderId,
        IPermissionGuard permissionGuard,
        CancellationToken cancellationToken = default)
        => Get(GetFilter(permissionGuard, folderId));

    public Task DeleteFromFoldersAsync(IEnumerable<string> folderIds, CancellationToken cancellationToken = default)
        => Delete(x => folderIds.Contains(x.FolderId));

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        => Delete(x => x.Id == id);

    private static Func<Note, bool> GetFilter(IPermissionGuard permissionGuard, string? folderId)
        => x => (folderId == null || x.FolderId == folderId)
           && permissionGuard.ExpressionCanView<Note>()
               .Compile().Invoke(x);
}
