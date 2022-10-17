using Notescrib.Api.Application.Common.Services;
using Notescrib.Api.Application.Notes.Models;
using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Enums;

namespace Notescrib.Api.Application.Workspaces.Mappers;

internal class WorkspaceMapper : IWorkspaceMapper
{
    public Workspace MapToEntity(AddWorkspace.Command command, string ownerId)
        => new()
        {
            Name = command.Name,
            SharingDetails = command.SharingDetails,
            OwnerId = ownerId,
        };

    public Workspace MapToEntity(UpdateWorkspace.Command command, Workspace old)
        => new()
        {
            Name = command.Name,
            SharingDetails = command.SharingDetails,

            Id = old.Id,
            OwnerId = old.OwnerId,
            Folders = old.Folders,
        };

    public WorkspaceDetails MapToResponse(Workspace workspace, IEnumerable<NoteOverview>? notes = null)
         => new()
         {
             Id = workspace.Id ?? string.Empty,
             Name = workspace.Name,
             SharingDetails = workspace.SharingDetails,
             OwnerId = workspace.OwnerId,
             Folders = workspace.Folders.Select(f => new FolderDetails
             {
                 Name = f.Name,
                 IsRoot = f.IsRoot,
                 ParentPath = f.AbsolutePath,
                 Notes = notes?.ToList() ?? new List<NoteOverview>()
             })
            .ToList()
         };        
}
