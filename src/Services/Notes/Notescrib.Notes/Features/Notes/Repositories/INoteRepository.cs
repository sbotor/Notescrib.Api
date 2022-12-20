using Notescrib.Notes.Features.Notes.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Repositories;

public interface INoteRepository
{
    Task<Note?> GetByIdAsync(string id, NoteIncludeOptions? include = null,
        CancellationToken cancellationToken = default);

    Task CreateAsync(NoteData note, CancellationToken cancellationToken = default);
    Task UpdateAsync(NoteData note, CancellationToken cancellationToken = default);
    Task UpdateContentAsync(NoteData note, CancellationToken cancellationToken = default);
    Task DeleteAsync(NoteBase note, CancellationToken cancellationToken = default);
    Task DeleteFromFoldersAsync(string ownerId, IEnumerable<string> folderIds, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Note>> GetByFolderIdAsync(string folderId, NoteIncludeOptions? include = null,
        CancellationToken cancellationToken = default);

    Task<PagedList<NoteBase>> SearchAsync(string ownerId, string? textFilter, bool ownOnly, PagingSortingInfo<NotesSorting> info,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<NoteBase>> GetManyAsync(IEnumerable<string> ids,
        CancellationToken cancellationToken = default);
}
