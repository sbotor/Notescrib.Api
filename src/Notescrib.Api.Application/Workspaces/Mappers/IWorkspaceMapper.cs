using AutoMapper;
using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Mappers;

public interface IWorkspaceMapper : IMapperBase
{
    Workspace CreateEntity(CreateWorkspace.Command command, string ownerId);
    Workspace UpdateEntity(UpdateWorkspace.Command command, Workspace old);
}
