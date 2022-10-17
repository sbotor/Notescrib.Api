using Notescrib.Api.Core.Contracts;

namespace Notescrib.Api.Core.Entities;

public class Note : EntityIdBase, IWorkspacePath
{
    public string Name { get; set; } = string.Empty;
    public string? ParentPath { get; set; }
    public string WorkspaceId { get; set; } = string.Empty;
    public NoteSection? RootSection { get; set; }
    public ICollection<string> Labels { get; set; } = new List<string>();
    public string OwnerId { get; set; } = string.Empty;
}
