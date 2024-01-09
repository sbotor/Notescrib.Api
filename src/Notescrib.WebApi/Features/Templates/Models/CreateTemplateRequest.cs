using Notescrib.Features.Templates.Commands;

namespace Notescrib.WebApi.Features.Templates.Models;

public class CreateTemplateRequest
{
    public string Name { get; set; } = null!;

    public CreateNoteTemplate.Command ToCommand()
        => new(Name);
}
