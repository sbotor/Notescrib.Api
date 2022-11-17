using Notescrib.Notes.Features.Workspaces.Commands;

namespace Notescrib.Notes.Api.Features.Workspaces.Models;

public class UpdateWorkspaceRequest
{
    public string Name { get; set; } = null!;

    public UpdateWorkspace.Command ToCommand(string id)
        => new(id, Name);
}
