namespace Notescrib.Notes.Features.Folders.Repositories;

public interface IFolderRepository
{
    Task CreateAsync(Folder folder, CancellationToken cancellationToken = default);

    Task<Folder?> GetByIdAsync(string id, FolderIncludeOptions? include = null,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(Folder folder, CancellationToken cancellationToken = default);
    Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    Task<Folder?> GetRootAsync(FolderIncludeOptions? include = null, CancellationToken cancellationToken = default);
    Task DeleteAllAsync(CancellationToken cancellationToken = default);
}
