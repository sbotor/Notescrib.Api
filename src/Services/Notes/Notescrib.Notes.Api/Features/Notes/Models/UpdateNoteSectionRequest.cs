using Notescrib.Notes.Features.Notes.Commands;

namespace Notescrib.Notes.Api.Features.Notes.Models;

public class UpdateNoteSectionRequest
{
    public string Name { get; set; } = null!;
    public string Content { get; set; } = null!;

    public UpdateNoteSection.Command ToCommand(string noteId, string sectionId)
        => new(noteId, sectionId, Name, Content);
}
