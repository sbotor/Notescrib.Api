using Notescrib.Notes.Application.Contracts;
using Notescrib.Notes.Application.Features.Workspaces.Models;

namespace Notescrib.Notes.Application.Features.Workspaces.Mappers;

public class WorkspaceMapper : IMapper<Workspace, WorkspaceOverview>
{
    public WorkspaceOverview Map(Workspace item)
        => new()
        {
            Id = item.Id,
            OwnerId = item.OwnerId,
            Name = item.Name,
            SharingInfo = item.SharingInfo
        };
}
