using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Workspaces.Models;

namespace Notescrib.Notes.Features.Workspaces.Mappers;

public class WorkspaceMapper : IMapper<Workspace, WorkspaceOverview>
{
    public WorkspaceOverview Map(Workspace item)
        => new() { Id = item.Id, OwnerId = item.OwnerId, Name = item.Name };
}
