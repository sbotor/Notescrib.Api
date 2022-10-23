using AutoMapper;
using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Mappers;

public interface IWorkspaceMapper : IMapperBase
{
    Workspace MapToEntity(AddWorkspace.Command command, string ownerId);
    Workspace MapToEntity(UpdateWorkspace.Command command, Workspace old);
}
