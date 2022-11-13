using Notescrib.Notes.Features.Workspaces.Commands;

namespace Notescrib.Notes.Api.Features.Workspaces.Models;

public class CreateWorkspaceRequest
{
    public string Name { get; set; } = null!;

    public CreateWorkspace.Command ToCommand()
        => new(Name);
}
