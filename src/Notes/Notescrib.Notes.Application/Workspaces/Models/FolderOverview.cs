using Notescrib.Notes.Core.Contracts;
using Notescrib.Notes.Core.Entities;

namespace Notescrib.Notes.Application.Workspaces.Models;

public class FolderOverview : IEntityId, IUpdatedTimestamp
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = null!;
    public DateTime Updated { get; set; }
}
