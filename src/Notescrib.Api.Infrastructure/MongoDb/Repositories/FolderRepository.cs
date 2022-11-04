using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Workspaces;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;
using Notescrib.Api.Infrastructure.MongoDb.Providers;

namespace Notescrib.Api.Infrastructure.MongoDb.Repositories;

internal class FolderRepository : MongoRepository<Folder>, IFolderRepository
{
    public FolderRepository(IMongoCollectionProvider collectionProvider, IDateTimeProvider dateTimeProvider)
        : base(collectionProvider, dateTimeProvider)
    {
    }

    public async Task<IReadOnlyCollection<Folder>> GetWorkspaceFoldersAsync(string workspaceId)
        => await GetAsync(x => x.WorkspaceId == workspaceId, new Sorting(nameof(Folder.ParentId)));
}
