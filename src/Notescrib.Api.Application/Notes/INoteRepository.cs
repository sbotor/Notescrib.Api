using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Notes;

public interface INoteRepository
{
    Task<string> AddNoteAsync(Note note);
    Task DeleteNoteAsync(string noteId);
    Task<Note?> GetNoteByIdAsync(string noteId);
    Task<PagedList<Note>> GetNotesFromTreeAsync(string folderId, IPaging paging);
    Task UpdateNoteAsync(Note note);
}
