using Notescrib.Api.Application.Common;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces;

public interface IWorkspaceRepository : IRepository<Workspace>
{
    Task<IPagedList<Workspace>> GetUserWorkspacesAsync(string ownerId, IPaging paging, ISorting? sorting = null);
}
