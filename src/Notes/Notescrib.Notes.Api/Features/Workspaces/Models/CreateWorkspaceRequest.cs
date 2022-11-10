using Notescrib.Notes.Features.Workspaces.Commands;
using Notescrib.Notes.Models;

namespace Notescrib.Notes.Api.Features.Workspaces.Models;

public class CreateWorkspaceRequest
{
    public string Name { get; set; } = null!;
    public SharingInfo? SharingInfo { get; set; }

    public CreateWorkspace.Command ToCommand()
        => new(Name, SharingInfo);
}
