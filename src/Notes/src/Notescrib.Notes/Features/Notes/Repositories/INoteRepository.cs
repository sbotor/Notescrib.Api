﻿using Notescrib.Notes.Features.Notes.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Repositories;

public interface INoteRepository
{
    Task<PagedList<Note>> GetNotesAsync(
        string? workspaceId,
        string? folderId,
        IPermissionGuard permissionGuard,
        PagingSortingInfo<NotesSorting> info,
        CancellationToken cancellationToken = default);

    Task AddNote(Note note, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string workspaceId,
        string folderId,
        string name,
        CancellationToken cancellationToken = default);

    Task<Note?> GetNoteByIdAsync(string id, CancellationToken cancellationToken = default);
    
    Task UpdateNoteAsync(Note note, CancellationToken cancellationToken = default);
    
    Task DeleteNotesFromWorkspaceAsync(string workspaceId, IEnumerable<string>? folderIds = null, CancellationToken cancellationToken = default);
}