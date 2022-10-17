using Notescrib.Api.Application.Workspaces.Commands;

namespace Notescrib.Api.Contracts.Workspaces;

public class AddFolderRequest
{
    public string Name { get; set; } = null!;
    public string? ParentPath { get; set; }

    public AddFolder.Command ToCommand(string workspaceId)
        => new(workspaceId, ParentPath, Name);
}
