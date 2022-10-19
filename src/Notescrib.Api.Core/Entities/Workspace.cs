using Notescrib.Api.Core.Contracts;

namespace Notescrib.Api.Core.Entities;

public class Workspace : EntityIdBase, IOwnableShareable, ICreatedTimestamp, IUpdatedTimestamp
{
    public string Name { get; set; } = string.Empty;
    public ICollection<Folder> Folders { get; set; } = new List<Folder>();

    public SharingDetails SharingDetails { get; set; } = new();
    public string OwnerId { get; set; } = string.Empty;

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}
