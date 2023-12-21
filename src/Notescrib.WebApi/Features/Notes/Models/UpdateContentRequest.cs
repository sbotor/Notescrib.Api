using Notescrib.Features.Notes.Commands;

namespace Notescrib.WebApi.Features.Notes.Models;

public class UpdateContentRequest
{
    public string Content { get; set; } = null!;

    public UpdateNoteContent.Command ToCommand(string id)
        => new(id, Content);
}
