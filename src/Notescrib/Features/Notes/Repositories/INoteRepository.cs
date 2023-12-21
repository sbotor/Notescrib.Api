using Notescrib.Features.Notes.Utils;
using Notescrib.Models;
using Notescrib.Utils;

namespace Notescrib.Features.Notes.Repositories;

public interface INoteRepository
{
    Task<Note?> GetByIdAsync(string id, NoteIncludeOptions? include = null,
        CancellationToken cancellationToken = default);

    Task CreateAsync(Note note, CancellationToken cancellationToken = default);
    Task UpdateAsync(Note note, CancellationToken cancellationToken = default);
    Task UpdateContentAsync(Note note, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Note>> GetByFolderIdAsync(string folderId, NoteIncludeOptions? include = null,
        CancellationToken cancellationToken = default);

    Task<PagedList<Note>> SearchAsync(string? ownerId, string? textFilter, bool ownOnly, PagingSortingInfo<NotesSorting> info,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Note>> GetManyAsync(IEnumerable<string> ids,
        CancellationToken cancellationToken = default);

    Task DeleteAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<string>> GetIdsFromFoldersAsync(IEnumerable<string> folderIds, CancellationToken cancellationToken = default);
    Task DeleteFromRelatedAsync(IEnumerable<string> noteIds, CancellationToken cancellationToken = default);
    Task DeleteFromRelatedAsync(string id, CancellationToken cancellationToken = default);
    Task UpdateRelatedAsync(Note note, CancellationToken cancellationToken = default);
}
