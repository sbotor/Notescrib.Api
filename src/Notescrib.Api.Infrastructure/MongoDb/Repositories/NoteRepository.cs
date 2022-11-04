using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Notes;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;
using Notescrib.Api.Infrastructure.MongoDb.Providers;

namespace Notescrib.Api.Infrastructure.MongoDb.Repositories;

internal class NoteRepository : MongoRepository<Note>, INoteRepository
{
    public NoteRepository(IMongoCollectionProvider collectionProvider, IDateTimeProvider dateTimeProvider)
        : base(collectionProvider, dateTimeProvider)
    {
    }

    public async Task<IPagedList<Note>> GetNotesFromFolderAsync(string folderId, IPaging paging, ISorting? sorting = null)
        => await GetPagedAsync(x => x.FolderId == folderId, paging, new Sorting(nameof(Note.Name)));
}
