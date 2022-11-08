using Notescrib.Notes.Core.Contracts;

namespace Notescrib.Notes.Core.Entities;

public class Workspace : EntityIdBase, IShareable, ICreatedTimestamp, IUpdatedTimestamp
{
    public string Name { get; set; } = null!;

    public SharingInfo SharingInfo { get; set; } = null!;
    public string OwnerId { get; set; } = null!;

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}
