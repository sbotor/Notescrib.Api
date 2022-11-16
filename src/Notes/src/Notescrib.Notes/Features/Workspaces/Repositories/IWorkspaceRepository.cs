using Notescrib.Notes.Features.Workspaces.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Workspaces.Repositories;

public interface IWorkspaceRepository
{
    Task<Workspace?> GetWorkspaceByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<PagedList<Workspace>> GetWorkspacesByOwnerIdAsync(
        string ownerId,
        PagingSortingInfo<WorkspacesSorting> info,
        CancellationToken cancellationToken = default);

    Task AddWorkspaceAsync(Workspace workspace, CancellationToken cancellationToken = default);
    Task UpdateWorkspaceAsync(Workspace workspace, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
}
