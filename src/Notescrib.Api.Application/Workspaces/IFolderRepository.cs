using Notescrib.Api.Application.Common;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces;

public interface IFolderRepository : IRepository<Folder>
{
    Task<IReadOnlyCollection<Folder>> GetWorkspaceFoldersAsync(string workspaceId);
}
