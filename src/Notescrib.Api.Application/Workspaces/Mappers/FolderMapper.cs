using Notescrib.Api.Application.Common.Mappers;
using Notescrib.Api.Application.Notes.Models;
using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Mappers;

internal class FolderMapper : MapperBase, IFolderMapper
{
    protected override void ConfigureMappings()
    {
        CreateMap<AddFolder.Command, Folder>();

        CreateMap<Folder, FolderDetails>();
        CreateMap<Folder, FolderOverview>();
    }

    public FolderDetails MapToDetails(Folder folder, IEnumerable<NoteOverview> notes)
    {
        var details = InternalMapper.Map<FolderDetails>(folder);
        details.Notes = notes.ToList();

        return details;
    }

    public IReadOnlyCollection<FolderOverview> MapToTree(IEnumerable<Folder> folders)
    {
        if (folders.Any())
        {
            return Array.Empty<FolderOverview>();
        }

        var grouped = folders.GroupBy(x => x.ParentId);
        var root = grouped.First(x => x.Key == null).First();

        return grouped.Select(x => GetOverviewRecursively(root, grouped)).ToList();
    }

    private FolderOverview GetOverviewRecursively(Folder parent, IEnumerable<IGrouping<string?, Folder>> groups)
    {
        var overview = InternalMapper.Map<FolderOverview>(parent);

        overview.Children = groups.First(x => x.Key == parent.Id)
            .Select(x => GetOverviewRecursively(x, groups))
            .ToList();

        return overview;
    }
}
