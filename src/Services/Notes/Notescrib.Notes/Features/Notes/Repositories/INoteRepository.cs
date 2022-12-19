namespace Notescrib.Notes.Features.Notes.Repositories;

public interface INoteRepository
{
    Task<Note?> GetByIdAsync(string id, NoteIncludeOptions? include = null,
        CancellationToken cancellationToken = default);

    Task CreateAsync(NoteData note, CancellationToken cancellationToken = default);
    Task UpdateAsync(NoteData note, CancellationToken cancellationToken = default);
    Task UpdateContentAsync(string id, string content, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task DeleteFromFoldersAsync(IEnumerable<string> folderIds, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Note>> GetByFolderIdAsync(string folderId, NoteIncludeOptions? include = null,
        CancellationToken cancellationToken = default);
}
