using Notescrib.Features.Folders.Commands;

namespace Notescrib.WebApi.Features.Folders.Models;

public class CreateFolderRequest
{
    public string Name { get; set; } = null!;
    public Guid? ParentId { get; set; } = null!;

    public CreateFolder.Command ToCommand()
        => new(Name, ParentId);
}
