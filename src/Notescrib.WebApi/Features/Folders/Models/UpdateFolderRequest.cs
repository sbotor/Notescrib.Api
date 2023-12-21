using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Features.Folders.Commands;
using Notescrib.Notes.Features.Workspaces;

namespace Notescrib.Notes.Api.Features.Folders.Models;

public class UpdateFolderRequest
{
    public string Name { get; set; } = null!;

    public UpdateFolder.Command ToCommand(string id)
        => new(id, Name);
}
