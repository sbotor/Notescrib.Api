using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Contracts.Workspace;

public class WorkspaceResponse
{
    public string Id { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public IReadOnlyCollection<FolderDetails> Folders { get; set; } = new List<FolderDetails>();
    public SharingDetails? SharingDetails { get; set; }
}
