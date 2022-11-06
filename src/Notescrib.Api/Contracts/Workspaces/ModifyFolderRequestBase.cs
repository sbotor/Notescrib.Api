using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Contracts.Workspaces;

public abstract class ModifyFolderRequestBase
{
    public string Name { get; set; } = null!;
    public string? ParentId { get; set; }
    public SharingInfo? SharingInfo { get; set; }
}
