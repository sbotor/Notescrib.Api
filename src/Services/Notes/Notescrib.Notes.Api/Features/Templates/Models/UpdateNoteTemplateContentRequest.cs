using Notescrib.Notes.Features.Templates.Commands;

namespace Notescrib.Notes.Api.Features.Templates.Models;

public class UpdateNoteTemplateContentRequest
{
    public string Content { get; set; } = null!;

    public UpdateNoteTemplateContent.Command ToCommand(string id)
        => new(id, Content);
}
