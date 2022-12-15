using Notescrib.Notes.Features.Workspaces;

namespace Notescrib.Notes.Features.Templates;

public class NoteTemplateBase
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public string WorkspaceId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}

public class NoteTemplate : NoteTemplateBase
{
    public Workspace Workspace { get; set; } = null!;
}
