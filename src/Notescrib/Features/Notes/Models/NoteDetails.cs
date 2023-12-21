namespace Notescrib.Features.Notes.Models;

public class NoteDetails : NoteOverview
{
    public string Content { get; set; } = null!;
    public IReadOnlyCollection<NoteOverview> Related { get; set; } = null!;
}
