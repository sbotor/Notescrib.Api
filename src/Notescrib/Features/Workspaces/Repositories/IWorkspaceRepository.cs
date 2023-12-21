namespace Notescrib.Notes.Features.Workspaces.Repositories;

public interface IWorkspaceRepository
{
    Task<Workspace?> GetByOwnerIdAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Workspace workspace, CancellationToken cancellationToken = default);
    Task DeleteAsync(string workspaceId, CancellationToken cancellationToken = default);
}
