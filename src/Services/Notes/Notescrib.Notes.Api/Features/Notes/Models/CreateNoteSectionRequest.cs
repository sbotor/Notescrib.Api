using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Notes.Commands;

namespace Notescrib.Notes.Api.Features.Notes.Models;

public class CreateNoteSectionRequest
{
    public string? ParentId { get; set; }
    public string Name { get; set; } = null!;

    public CreateNoteSection.Command ToCommand(string noteId)
        => new(noteId, ParentId ?? NoteSection.RootId, Name);
}
