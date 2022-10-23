using MongoDB.Bson;
using MongoDB.Driver;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Notes;

internal class NoteRepository : INoteRepository
{
    private readonly IPersistenceProvider<Note> _notes;

    public NoteRepository(IPersistenceProvider<Note> persistenceProvider)
    {
        _notes = persistenceProvider;
    }

    public async Task<string> AddNoteAsync(Note note)
        => await _notes.AddAsync(note);

    public async Task<Note?> GetNoteByIdAsync(string noteId)
        => await _notes.FindByIdAsync(noteId);

    public async Task UpdateNoteAsync(Note note)
        => await _notes.UpdateAsync(note);

    public async Task DeleteNoteAsync(string noteId)
        => await _notes.DeleteAsync(noteId);

    public async Task<IPagedList<Note>> GetNotesFromTreeAsync(string folderId, IPaging paging)
    {
        var field = new StringFieldDefinition<Note>(nameof(Note.ParentPath));
        var regex = new BsonRegularExpression($"{folderId}.*");

        var filter = Builders<Note>.Filter.Regex(field, regex);

        return await _notes.FindPagedAsync(filter, paging, new Sorting(nameof(Note.ParentPath)));
    }
}
