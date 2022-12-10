using Notescrib.Notes.Features.Notes.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Repositories;

public interface INoteRepository
{
    Task<PagedList<Note>> GetAsync(
        string? folderId,
        IPermissionGuard permissionGuard,
        PagingSortingInfo<NotesSorting> info,
        CancellationToken cancellationToken = default);

    Task AddNote(Note note, CancellationToken cancellationToken = default);

    Task<Note?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(Note note, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Note>> GetAsync(
        string? folderId,
        IPermissionGuard permissionGuard,
        CancellationToken cancellationToken = default);

    Task DeleteFromFoldersAsync(IEnumerable<string> folderIds,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
