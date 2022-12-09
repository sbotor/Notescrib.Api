using Notescrib.Notes.Features.Folders.Commands;
using Notescrib.Notes.Features.Workspaces;

namespace Notescrib.Notes.Api.Features.Folders.Models;

public class UpdateFolderRequest
{
    public string Name { get; set; } = null!;
    public string? ParentId { get; set; }

    public UpdateFolder.Command ToCommand(string id)
        => new(id, Name, ParentId ?? Folder.RootId);
}
