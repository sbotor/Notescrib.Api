using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Tests.Infrastructure;

namespace Notescrib.Notes.Tests.Features.Workspaces;

public class TestWorkspaceRepository : TestRepositoryBase<Workspace>, IWorkspaceRepository
{
    public Task<Workspace?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        => GetSingleOrDefault(x => x.Id == id);

    public Task AddAsync(Workspace workspace, CancellationToken cancellationToken = default)
        => Add(workspace, w => w.Id = Guid.NewGuid().ToString());

    public Task UpdateAsync(Workspace workspace, CancellationToken cancellationToken = default)
        => Update(workspace, x => x.Id == workspace.Id);

    public Task DeleteAsync(string workspaceId, CancellationToken cancellationToken = default)
        => Delete(x => x.Id == workspaceId);

    public Task<Workspace?> GetByOwnerIdAsync(string ownerId, CancellationToken cancellationToken = default)
        => GetSingleOrDefault(x => x.OwnerId == ownerId); 

    public Task<long> CountAsync(string ownerId, CancellationToken cancellationToken = default)
        => Count(x => x.OwnerId == ownerId);
}
