using Notescrib.Notes.Application.Models;

namespace Notescrib.Notes.Application.Features.Workspaces.Models;

public class WorkspaceOverview
{
    public string Id { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = null!;
}
