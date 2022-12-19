namespace Notescrib.Notes.Features.Folders.Repositories;

public interface IFolderRepository
{
    Task AddAsync(FolderData folder, CancellationToken cancellationToken = default);

    Task<Folder?> GetByIdAsync(string id, FolderIncludeOptions? include = null,
        CancellationToken cancellationToken = default);
    Task UpdateAsync(FolderData folder, CancellationToken cancellationToken = default);
    Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    Task<Folder?> GetRootAsync(string ownerId, FolderIncludeOptions? include = null, CancellationToken cancellationToken = default);
}
