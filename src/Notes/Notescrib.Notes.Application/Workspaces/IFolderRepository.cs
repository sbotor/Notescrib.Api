using Notescrib.Notes.Application.Common.Contracts;
using Notescrib.Notes.Core.Entities;

namespace Notescrib.Notes.Application.Workspaces;

public interface IFolderRepository : IRepository<Folder>
{
    Task<IReadOnlyCollection<Folder>> GetWorkspaceFoldersAsync(string workspaceId);
}
