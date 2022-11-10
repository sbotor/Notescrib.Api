using Notescrib.Notes.Features.Workspaces.Commands;
using Notescrib.Notes.Models;

namespace Notescrib.Notes.Api.Features.Workspaces.Models;

public class CreateFolderRequest
{
    public string Name { get; set; } = null!;
    public string? Parent { get; set; } = null!;
    public SharingInfo? SharingInfo { get; set; } = null!;

    public CreateFolder.Command ToCommand(string workspaceId)
        => new(workspaceId, Name, Parent, SharingInfo);
}
