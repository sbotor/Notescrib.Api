using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Workspaces;
using Notescrib.Api.Infrastructure.MongoDb.Providers;

namespace Notescrib.Api.Infrastructure.MongoDb.Repositories;

internal class WorkspaceRepository : MongoRepository<Workspace>, IWorkspaceRepository
{
    public WorkspaceRepository(IMongoCollectionProvider collectionProvider, IDateTimeProvider dateTimeProvider)
        : base(collectionProvider, dateTimeProvider)
    {
    }

    public async Task<IPagedList<Workspace>> GetUserWorkspacesAsync(string ownerId, IPaging paging, ISorting? sorting = null)
        => await GetPagedAsync(x => x.OwnerId == ownerId, paging, sorting);
}
