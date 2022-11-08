using Notescrib.Notes.Application.Common.Contracts;
using Notescrib.Notes.Core.Contracts;
using Notescrib.Notes.Core.Entities;

namespace Notescrib.Notes.Application.Workspaces;

public interface IWorkspaceRepository : IRepository<Workspace>
{
    Task<IPagedList<Workspace>> GetUserWorkspacesAsync(string ownerId, IPaging paging, ISorting? sorting = null);
}
