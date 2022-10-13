using System.Linq.Expressions;
using MongoDB.Driver;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Infrastructure.MongoDb.Providers;

namespace Notescrib.Api.Infrastructure.MongoDb.Repositories;

internal class NoteRepository
{
    private readonly IMongoPersistenceProvider<Note> _persistenceProvider;

    public NoteRepository(IMongoPersistenceProvider<Note> persistenceProvider)
    {
        _persistenceProvider = persistenceProvider;
    }

    public async Task<Note> AddNoteAsync(Note note)
        => await _persistenceProvider.AddAsync(note);

    public async Task<Note?> GetNoteByIdAsync(string noteId)
        => await _persistenceProvider.FindByIdAsync(noteId);

    public async Task UpdateNoteAsync(Note note)
        => await _persistenceProvider.UpdateAsync(note);

    public async Task<bool> DeleteNoteAsync(string noteId)
        => await _persistenceProvider.DeleteAsync(noteId);

    public async Task<IReadOnlyCollection<Note>> GetNotesFromFolderAsync(string path, bool includeChildren = true)
    {
        Expression<Func<Note, bool>> filter = includeChildren
            ? x => x.FullParentPath.StartsWith(path)
            : x => x.FullParentPath == path;

        var result = await _persistenceProvider.Collection.FindAsync(filter);
        return await result.ToListAsync();
    }
}
