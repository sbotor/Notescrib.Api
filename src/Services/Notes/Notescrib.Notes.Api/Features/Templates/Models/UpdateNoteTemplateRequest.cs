using Notescrib.Notes.Features.Templates.Commands;

namespace Notescrib.Notes.Api.Features.Templates.Models;

public class UpdateNoteTemplateRequest
{
    public string Name { get; set; } = null!;

    public UpdateNoteTemplate.Command ToCommand(string id)
        => new(id, Name);
}
