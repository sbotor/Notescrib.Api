using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Exceptions;

namespace Notescrib.Api.Core.Entities;

public class Workspace : EntityIdBase, IOwnableShareable, ICreatedTimestamp, IUpdatedTimestamp
{
    public string Name { get; set; } = null!;

    public SharingDetails SharingDetails { get; set; } = null!;
    public string OwnerId { get; set; } = null!;

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}
