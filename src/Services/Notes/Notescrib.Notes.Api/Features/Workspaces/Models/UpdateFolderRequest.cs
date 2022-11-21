using Notescrib.Notes.Features.Workspaces.Commands;

namespace Notescrib.Notes.Api.Features.Workspaces.Models;

public class UpdateFolderRequest
{
    public string Name { get; set; } = null!;
    public string? ParentId { get; set; }

    public UpdateFolder.Command ToCommand(string workspaceId, string id)
        => new(workspaceId, id, Name, ParentId);
}
