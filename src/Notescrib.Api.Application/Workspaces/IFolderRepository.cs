using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces;
internal interface IFolderRepository
{
    Task<string> AddFolderAsync(Folder folder);
    Task DeleteFolderAsync(string id);
    Task<Folder?> GetFolderByIdAsync(string id);
    Task<IReadOnlyCollection<Folder>> GetWorkspaceFoldersAsync(string workspaceId);
    Task UpdateFolderAsync(Folder folder);
}
