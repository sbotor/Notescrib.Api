using Notescrib.Features.Templates.Commands;

namespace Notescrib.WebApi.Features.Templates.Models;

public class UpdateNoteTemplateRequest
{
    public string Name { get; set; } = null!;

    public UpdateNoteTemplate.Command ToCommand(string id)
        => new(id, Name);
}
