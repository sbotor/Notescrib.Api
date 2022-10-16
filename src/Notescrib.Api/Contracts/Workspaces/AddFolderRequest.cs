using Notescrib.Api.Application.Workspaces.Commands;

namespace Notescrib.Api.Contracts.Workspaces;

public class AddFolderRequest
{
    public string Name { get; set; } = string.Empty;
    public string ParentFolderPath { get; set; } = string.Empty;

    public AddFolder.Command ToCommand(string workspaceId)
        => new(workspaceId, ParentFolderPath, Name);
}
