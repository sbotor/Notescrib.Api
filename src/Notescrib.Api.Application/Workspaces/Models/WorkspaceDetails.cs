using Notescrib.Api.Core.Contracts;

namespace Notescrib.Api.Application.Workspaces.Models;

public class WorkspaceDetails : WorkspaceOverview
{
    public IPagedList<FolderOverview> Folders { get; set; } = null!;
}
