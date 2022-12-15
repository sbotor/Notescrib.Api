namespace Notescrib.Notes.Features.Notes.Repositories;

public interface INoteContentRepository
{
    Task<NoteContent?> GetByNoteIdAsync(string noteId, CancellationToken cancellationToken = default);
    Task CreateAsync(string noteId, CancellationToken cancellationToken = default);
    Task UpdateAsync(NoteContentData content, CancellationToken cancellationToken = default);
    Task DeleteAsync(string noteId, CancellationToken cancellationToken = default);
    Task DeleteManyAsync(IEnumerable<string> noteIds, CancellationToken cancellationToken = default);
}
