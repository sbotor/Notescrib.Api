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
        CreateMap<CreateFolder.Command, Folder>();
        CreateMap<UpdateFolder.Command, Folder>();

        CreateMap<Folder, FolderDetails>();
        CreateMap<Folder, FolderOverview>();
    }

    public FolderDetails MapToDetails(Folder folder, IEnumerable<NoteOverview> notes)
    {
        var details = InternalMapper.Map<FolderDetails>(folder);
        details.Notes = notes.ToList();

        return details;
    }

    public Folder Update(Folder original, UpdateFolder.Command command)
    {
        var folder = InternalMapper.Map(command, original);

        // folder.Id = original.Id;
        // folder.WorkspaceId = original.WorkspaceId;
        // folder.OwnerId = original.OwnerId;

        return folder;
    }
}
