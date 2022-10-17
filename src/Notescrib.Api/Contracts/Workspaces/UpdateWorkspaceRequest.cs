using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Contracts.Workspaces;

public class UpdateWorkspaceRequest
{
    public string Name { get; set; } = null!;
    public SharingDetails SharingDetails { get; set; } = null!;

    public UpdateWorkspace.Command ToCommand(string id)
        => new(id, Name, SharingDetails);
}
