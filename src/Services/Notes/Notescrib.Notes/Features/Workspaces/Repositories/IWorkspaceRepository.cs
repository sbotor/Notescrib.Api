namespace Notescrib.Notes.Features.Workspaces.Repositories;

public interface IWorkspaceRepository
{
    Task<Workspace?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Workspace?> GetByOwnerIdAsync(string ownerId, CancellationToken cancellationToken = default);
    Task AddAsync(Workspace workspace, CancellationToken cancellationToken = default);
    Task UpdateAsync(Workspace workspace, CancellationToken cancellationToken = default);
    Task DeleteAsync(string workspaceId, CancellationToken cancellationToken = default);
}
