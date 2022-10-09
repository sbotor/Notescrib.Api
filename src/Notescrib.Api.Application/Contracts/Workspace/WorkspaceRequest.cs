using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Contracts.Workspace;

public class WorkspaceRequest
{
    public string Name { get; set; } = null!;
    public SharingDetails? SharingDetails { get; set; }
}
