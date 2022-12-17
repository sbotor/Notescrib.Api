namespace Notescrib.Notes.Features.Notes.Models;

public class NoteDetails : NoteOverview
{
    public string Content { get; set; } = null!;
    public bool IsReadonly { get; set; }
}
