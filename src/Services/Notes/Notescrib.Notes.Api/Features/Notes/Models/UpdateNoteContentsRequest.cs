using Notescrib.Notes.Features.Notes.Commands;
using Notescrib.Notes.Features.Notes.Models;

namespace Notescrib.Notes.Api.Features.Notes.Models;

public class UpdateNoteContentsRequest
{
    public IReadOnlyCollection<NoteContentsSection> Sections { get; set; } = null!;

    public UpdateNoteContents.Command ToCommand(string noteId)
        => new(noteId, Sections);
}
