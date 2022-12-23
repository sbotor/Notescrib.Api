using Notescrib.Notes.Features.Notes.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Repositories;

public interface INoteRepository
{
    Task<Note?> GetByIdAsync(string id, NoteIncludeOptions? include = null,
        CancellationToken cancellationToken = default);

    Task CreateAsync(Note note, CancellationToken cancellationToken = default);
    Task UpdateAsync(Note note, CancellationToken cancellationToken = default);
    Task UpdateContentAsync(Note note, CancellationToken cancellationToken = default);
    Task DeleteAsync(Note note, CancellationToken cancellationToken = default);
    Task DeleteFromFoldersAsync(string ownerId, IEnumerable<string> folderIds, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Note>> GetByFolderIdAsync(string folderId, NoteIncludeOptions? include = null,
        CancellationToken cancellationToken = default);

    Task<PagedList<Note>> SearchAsync(string? ownerId, string? textFilter, bool ownOnly, PagingSortingInfo<NotesSorting> info,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Note>> GetManyAsync(IEnumerable<string> ids,
        CancellationToken cancellationToken = default);

    Task DeleteAllAsync(string workspaceId, CancellationToken cancellationToken = default);
}
