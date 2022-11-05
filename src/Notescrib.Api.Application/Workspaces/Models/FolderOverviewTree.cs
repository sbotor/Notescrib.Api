using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Models;

public class FolderOverviewTree : FolderTreeBase<FolderOverview, Folder>
{
    internal FolderOverviewTree(IEnumerable<Folder> items, IFolderMapper mapper)
        : base(items, x => mapper.Map<FolderOverview>(x))
    {
    }
}
