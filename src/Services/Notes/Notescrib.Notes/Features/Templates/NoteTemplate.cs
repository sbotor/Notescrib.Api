namespace Notescrib.Notes.Features.NoteTemplates;

public class NoteTemplate
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public string WorkspaceId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}
