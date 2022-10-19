using AutoMapper;
using Notescrib.Api.Application.Common.Mappers;
using Notescrib.Api.Application.Notes.Models;
using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Mappers;

internal class WorkspaceMapper : MapperBase, IWorkspaceMapper
{
    private readonly IFolderMapper _folderMapper;

    public WorkspaceMapper(IFolderMapper folderMapper)
    {
        _folderMapper = folderMapper;
    }

    public Workspace MapToEntity(AddWorkspace.Command command, string ownerId)
    {
        var workspace = InternalMapper.Map<Workspace>(command);
        workspace.OwnerId = ownerId;

        return workspace;
    }

    public Workspace MapToEntity(UpdateWorkspace.Command command, Workspace old)
    {
        var workspace = InternalMapper.Map<Workspace>(command);

        workspace.Id = old.Id;
        workspace.OwnerId = old.OwnerId;
        workspace.Folders = old.Folders;

        return workspace;
    }

    public WorkspaceDetails MapToResponse(Workspace workspace, IEnumerable<NoteOverview>? notes = null)
    {
        var details = InternalMapper.Map<WorkspaceDetails>(workspace);

        details.Folders = workspace.Folders
            .Select(f => _folderMapper.MapToResponse(f, notes ?? Enumerable.Empty<NoteOverview>()))
            .ToList();

        return details;
    }
}
