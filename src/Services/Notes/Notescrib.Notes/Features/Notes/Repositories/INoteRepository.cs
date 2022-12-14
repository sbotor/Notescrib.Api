namespace Notescrib.Notes.Features.Notes.Repositories;

public interface INoteRepository
{
    Task CreateNote(NoteBase note, CancellationToken cancellationToken = default);
    Task<Note?> GetByIdAsync(string id, NoteIncludeOptions? include = null, CancellationToken cancellationToken = default);
    Task UpdateAsync(NoteBase note, CancellationToken cancellationToken = default);

    Task<int> DeleteFromFoldersAsync(IEnumerable<string> folderIds,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task UpdateContentAsync(NoteBase note, CancellationToken cancellationToken = default);
}
