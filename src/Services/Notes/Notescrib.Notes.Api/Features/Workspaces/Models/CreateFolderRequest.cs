using Notescrib.Notes.Features.Workspaces.Commands;

namespace Notescrib.Notes.Api.Features.Workspaces.Models;

public class CreateFolderRequest
{
    public string Name { get; set; } = null!;
    public string? Parent { get; set; } = null!;

    public CreateFolder.Command ToCommand(string workspaceId)
        => new(workspaceId, Name, Parent);
}
