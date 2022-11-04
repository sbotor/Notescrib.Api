using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Contracts.Workspaces;

public class AddFolderRequest
{
    public string Name { get; set; } = null!;
    public string? ParentId { get; set; }
    public SharingInfo? SharingInfo { get; set; }

    public AddFolder.Command ToCommand(string workspaceId)
        => new(workspaceId, ParentId, Name, SharingInfo);
}
