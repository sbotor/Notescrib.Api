using Notescrib.Api.Core.Contracts;

namespace Notescrib.Api.Core.Entities;

public class Note : EntityIdBase, IWorkspacePath, IOwnableShareable, ICreatedTimestamp, IUpdatedTimestamp
{
    public string Name { get; set; } = string.Empty;
    public string? ParentPath { get; set; }
    public string WorkspaceId { get; set; } = string.Empty;
    public NoteSection? RootSection { get; set; }
    public ICollection<string> Labels { get; set; } = new List<string>();

    public string OwnerId { get; set; } = string.Empty;
    public SharingDetails SharingDetails { get; set; } = new();

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}
