﻿using System.Linq.Expressions;
using MongoDB.Driver;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Helpers;
using Notescrib.Api.Core.Models;
using Notescrib.Api.Infrastructure.MongoDb.Providers;

namespace Notescrib.Api.Infrastructure.MongoDb.Repositories;

internal class NoteRepository
{
    private readonly IMongoPersistenceProvider<Note> _notes;

    public NoteRepository(IMongoPersistenceProvider<Note> persistenceProvider)
    {
        _notes = persistenceProvider;
    }

    public async Task<Note> AddNoteAsync(Note note)
        => await _notes.AddAsync(note);

    public async Task<Note?> GetNoteByIdAsync(string noteId)
        => await _notes.FindByIdAsync(noteId);

    public async Task UpdateNoteAsync(Note note)
        => await _notes.UpdateAsync(note);

    public async Task<bool> DeleteNoteAsync(string noteId)
        => await _notes.DeleteAsync(noteId);

    public async Task<PagedList<Note>> GetNotesFromFolderAsync(
        IWorkspacePath path,
        IPaging paging,
        ISorting? sorting = null,
        bool includeChildrenFolders = true)
    {
        var comparer = new WorkspacePathComparer();

        Expression<Func<Note, bool>> filter = includeChildrenFolders
            ? x => comparer.StartsWith(x, path)
            : x => comparer.Equals(x, path);

        return await _notes.FindPagedAsync(filter, paging, sorting);
    }
}
