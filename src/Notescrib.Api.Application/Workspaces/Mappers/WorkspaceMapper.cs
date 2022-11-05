using Notescrib.Api.Application.Common.Mappers;
using Notescrib.Api.Application.Extensions;
using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Mappers;

internal class WorkspaceMapper : MapperBase, IWorkspaceMapper
{
    protected override void ConfigureMappings()
    {
        CreateMap<CreateWorkspace.Command, Workspace>();
        CreateMap<UpdateWorkspace.Command, Workspace>()
            .Ignore(x => x.Id).Ignore(x => x.OwnerId);

        CreateMap<Workspace, WorkspaceOverview>();
        CreateMap<Workspace, WorkspaceDetails>();
    }

    public Workspace CreateEntity(CreateWorkspace.Command command, string ownerId)
    {
        var workspace = InternalMapper.Map<Workspace>(command);
        workspace.OwnerId = ownerId;

        return workspace;
    }

    public Workspace UpdateEntity(UpdateWorkspace.Command command, Workspace old)
    {
        var workspace = InternalMapper.Map(command, old);

        workspace.Id = old.Id;
        workspace.OwnerId = old.OwnerId;

        return workspace;
    }
}
