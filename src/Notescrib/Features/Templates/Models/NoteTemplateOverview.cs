namespace Notescrib.Features.Templates.Models;

public class NoteTemplateOverview
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; } 
}
