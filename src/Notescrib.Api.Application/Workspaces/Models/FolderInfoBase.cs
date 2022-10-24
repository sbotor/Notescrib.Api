using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Models;

public abstract class FolderInfoBase : IShareable
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public SharingDetails SharingDetails { get; set; } = null!;
}
