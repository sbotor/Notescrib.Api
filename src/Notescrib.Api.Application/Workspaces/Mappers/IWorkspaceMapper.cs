using AutoMapper;
using Notescrib.Api.Application.Notes.Models;
using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Mappers;

public interface IWorkspaceMapper
{
    public IMapper InternalMapper { get; }

    Workspace MapToEntity(AddWorkspace.Command command, string ownerId);
    Workspace MapToEntity(UpdateWorkspace.Command command, Workspace old);
    WorkspaceDetails MapToResponse(Workspace workspace, IEnumerable<NoteOverview>? notes = null);
}
