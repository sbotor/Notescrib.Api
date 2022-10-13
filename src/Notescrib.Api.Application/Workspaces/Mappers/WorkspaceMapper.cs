using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Application.Workspaces.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Mappers;

internal class WorkspaceMapper
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
            Name = command.Name ?? old.Name,
            SharingDetails = command.SharingDetails ?? old.SharingDetails,

            Id = old.Id,
            OwnerId = old.OwnerId,
            Folders = old.Folders,
        };

    public WorkspaceResponse MapToResponse(Workspace workspace)
         => new()
         {
             Id = workspace.Id ?? string.Empty,
             Name = workspace.Name,
             SharingDetails = workspace.SharingDetails,
             OwnerId = workspace.OwnerId,
             Folders = workspace.Folders.Select(f => new FolderResponse
             {
                 Name = f.Name,
                 IsRoot = f.IsRoot,
                 Path = f.AbsolutePath,
             })
            .ToList()
         };
}
