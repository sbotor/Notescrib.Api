using Notescrib.Api.Application.Common.Mappers;
using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Mappers;

internal class WorkspaceMapper : MapperBase, IWorkspaceMapper
{
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

        return workspace;
    }
}
