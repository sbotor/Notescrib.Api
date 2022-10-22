using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Contracts.Workspaces;

public class AddFolderRequest
{
    public string Name { get; set; } = null!;
    public string? ParentPath { get; set; }
    public SharingDetails? SharingDetails { get; set; }

    public AddFolder.Command ToCommand(string workspaceId)
        => new(workspaceId, ParentPath, Name, SharingDetails);
}
