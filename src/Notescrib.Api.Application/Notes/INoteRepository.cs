using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Notes;

public interface INoteRepository
{
    Task<string> AddNoteAsync(Note note);
    Task DeleteNoteAsync(string noteId);
    Task<Note?> GetNoteByIdAsync(string noteId);
    Task<IPagedList<Note>> GetNotesFromFolderAsync(string folderId, IPaging paging, ISorting? sorting = null);
    Task UpdateNoteAsync(Note note);
}
