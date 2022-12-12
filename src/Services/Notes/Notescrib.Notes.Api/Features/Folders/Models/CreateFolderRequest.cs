using Notescrib.Notes.Features.Folders.Commands;

namespace Notescrib.Notes.Api.Features.Folders.Models;

public class CreateFolderRequest
{
    public string Name { get; set; } = null!;
    public string? ParentId { get; set; } = null!;

    public CreateFolder.Command ToCommand()
        => new(Name, ParentId);
}
