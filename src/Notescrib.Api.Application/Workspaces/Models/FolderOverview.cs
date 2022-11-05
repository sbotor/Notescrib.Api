using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Models;

public class FolderOverview : IEntityId
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = null!;
}
