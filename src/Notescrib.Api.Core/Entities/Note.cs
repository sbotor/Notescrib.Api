using Notescrib.Api.Core.Contracts;

namespace Notescrib.Api.Core.Entities;

public class Note : EntityIdBase, IOwnableShareable, ICreatedTimestamp, IUpdatedTimestamp
{
    public string Name { get; set; } = null!;
    public string FolderId { get; set; } = null!;
    public string WorkspaceId { get; set; } = null!;
    public NoteSection? RootSection { get; set; }
    public ICollection<string> Labels { get; set; } = null!;

    public string OwnerId { get; set; } = null!;
    public SharingDetails SharingDetails { get; set; } = null!;

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}
