using Notescrib.Notes.Features.Templates.Commands;

namespace Notescrib.Notes.Api.Features.Templates.Models;

public class CreateTemplateRequest
{
    public string Name { get; set; } = null!;

    public CreateNoteTemplate.Command ToCommand()
        => new(Name);
}
