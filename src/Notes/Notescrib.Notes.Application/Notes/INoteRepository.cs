using Notescrib.Notes.Application.Common.Contracts;
using Notescrib.Notes.Core.Contracts;
using Notescrib.Notes.Core.Entities;

namespace Notescrib.Notes.Application.Notes;

public interface INoteRepository : IRepository<Note>
{
    Task<IPagedList<Note>> GetNotesFromFolderAsync(string folderId, IPaging paging, ISorting? sorting = null);
}
