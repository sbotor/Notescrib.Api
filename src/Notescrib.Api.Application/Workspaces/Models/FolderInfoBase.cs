using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Models;

public abstract class FolderInfoBase
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = null!;
}
