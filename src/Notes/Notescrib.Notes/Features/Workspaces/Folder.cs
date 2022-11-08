using Notescrib.Notes.Application.Models;

namespace Notescrib.Notes.Application.Features.Workspaces;

public class Folder
{
    public string Name { get; set; } = null!;
    public string WorkspaceId { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = null!;
    public ICollection<Folder> Children { get; set; } = new List<Folder>();
}
