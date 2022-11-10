using Notescrib.Notes.Features.Workspaces.Utils;
using Notescrib.Notes.Models;

namespace Notescrib.Notes.Features.Workspaces;

public class Workspace
{
    public string Id { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = null!;
    public FolderTree FolderTree { get; set; } = new();
}
