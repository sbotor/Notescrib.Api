using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Contracts;

public class WorkspaceResponse
{
    public string Id { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public IReadOnlyCollection<FolderResponse> Folders { get; set; } = new List<FolderResponse>();
    public SharingDetails? SharingDetails { get; set; }
}
