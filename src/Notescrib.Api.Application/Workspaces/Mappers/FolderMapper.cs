using Notescrib.Api.Application.Common.Mappers;
using Notescrib.Api.Application.Notes.Models;
using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Mappers;

internal class FolderMapper : MapperBase, IFolderMapper
{
    public FolderDetails MapToDetails(Folder folder, IEnumerable<NoteOverview> notes)
    {
        var details = InternalMapper.Map<FolderDetails>(folder);
        details.Notes = notes.ToList();

        return details;
    }

    protected override void ConfigureMappings()
    {
        CreateMap<Folder, FolderDetails>();

        CreateMap<AddFolder.Command, Folder>();
    }
}
