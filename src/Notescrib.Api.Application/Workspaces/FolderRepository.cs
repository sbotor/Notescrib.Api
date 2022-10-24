using Notescrib.Api.Application.Common;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces;

internal class FolderRepository : IFolderRepository
{
    private readonly IPersistenceProvider<Folder> _folders;

    public FolderRepository(IPersistenceProvider<Folder> persistenceProvider)
    {
        _folders = persistenceProvider;
    }

    public async Task<Folder?> GetFolderByIdAsync(string id)
        => await _folders.FindByIdAsync(id);

    public async Task<string> AddFolderAsync(Folder folder)
        => await _folders.AddAsync(folder);

    public async Task UpdateFolderAsync(Folder folder)
        => await _folders.UpdateAsync(folder);

    public async Task DeleteFolderAsync(string id)
        => await _folders.DeleteAsync(id);

    public async Task<IReadOnlyCollection<Folder>> GetWorkspaceFoldersAsync(string workspaceId)
        => await _folders.FindAsync(x => x.WorkspaceId == workspaceId, new Sorting(nameof(Folder.ParentId)));
}
