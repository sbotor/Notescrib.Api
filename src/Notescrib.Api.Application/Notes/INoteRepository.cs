using Notescrib.Api.Application.Common;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Notes;

public interface INoteRepository : IRepository<Note>
{
    Task<IPagedList<Note>> GetNotesFromFolderAsync(string folderId, IPaging paging, ISorting? sorting = null);
}
