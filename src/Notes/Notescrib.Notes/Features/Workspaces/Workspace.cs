using Notescrib.Notes.Application.Models;

namespace Notescrib.Notes.Application.Features.Workspaces;

public class Workspace
{
    public string Id { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = null!;
    public ICollection<Folder> Folders { get; set; } = new List<Folder>();
}
