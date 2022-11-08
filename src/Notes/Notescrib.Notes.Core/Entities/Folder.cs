using Notescrib.Notes.Core.Contracts;

namespace Notescrib.Notes.Core.Entities;

public class Folder : EntityIdBase, IShareable, ICreatedTimestamp, IUpdatedTimestamp, IFolderStructure
{
    public const string Separator = "/";

    public string Name { get; set; } = null!;
    public string? ParentId { get; set; }
    public string WorkspaceId { get; set; } = null!;

    public string OwnerId { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = null!;

    public bool IsRoot => ParentId == null;

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}
