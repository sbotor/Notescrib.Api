using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Features.Workspaces.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Tests.Infrastructure;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Tests.Features.Workspaces;

public class TestWorkspaceRepository : TestRepositoryBase<Workspace, WorkspacesSorting>, IWorkspaceRepository
{
    public Task<Workspace?> GetWorkspaceByIdAsync(string id, CancellationToken cancellationToken = default)
        => GetSingleOrDefault(x => x.Id == id);

    public Task<PagedList<Workspace>> GetWorkspacesByOwnerIdAsync(
        string ownerId,
        PagingSortingInfo<WorkspacesSorting> info,
        CancellationToken cancellationToken = default)
        => GetPaged(x => x.OwnerId == ownerId, info);

    public Task AddWorkspaceAsync(Workspace workspace, CancellationToken cancellationToken = default)
        => Add(workspace, w => w.Id = Guid.NewGuid().ToString());

    public Task UpdateWorkspaceAsync(Workspace workspace, CancellationToken cancellationToken = default)
        => Update(workspace, x => x.Id == workspace.Id);

    public Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
        => Exists(x => x.Name == name);
}
