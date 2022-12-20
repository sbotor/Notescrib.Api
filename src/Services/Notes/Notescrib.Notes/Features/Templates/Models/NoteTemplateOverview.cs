namespace Notescrib.Notes.Features.Templates.Models;

public class NoteTemplateOverview
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; } 
}
