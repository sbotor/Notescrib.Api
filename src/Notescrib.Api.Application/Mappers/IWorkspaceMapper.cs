using Notescrib.Api.Application.Contracts.Workspace;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Mappers;
internal interface IWorkspaceMapper
{
    Workspace MapToEntity(WorkspaceRequest request, string ownerId, Workspace? original = null);
    WorkspaceResponse MapToResponse(Workspace workspace, IEnumerable<NoteDetails>? notes = null);
}
