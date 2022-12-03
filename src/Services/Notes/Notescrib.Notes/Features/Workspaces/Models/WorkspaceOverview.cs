using Notescrib.Notes.Models;

namespace Notescrib.Notes.Features.Workspaces.Models;

public class WorkspaceOverview
{
    public string Id { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public string Name { get; set; } = null!;
    
    public DateTime Created { get; set; }
    public DateTime? Edited { get; set; }
}
