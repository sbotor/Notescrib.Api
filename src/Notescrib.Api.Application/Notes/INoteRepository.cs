using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Notes;

public interface INoteRepository
{
    Task<Note> AddNoteAsync(Note note);
    Task<bool> DeleteNoteAsync(string noteId);
    Task<Note?> GetNoteByIdAsync(string noteId);
    Task<PagedList<Note>> GetNotesFromFolderAsync(IWorkspacePath path, IPaging paging, ISorting? sorting = null, bool includeChildrenFolders = true);
    Task UpdateNoteAsync(Note note);
}
