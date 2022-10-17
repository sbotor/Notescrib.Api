using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Models;

public class WorkspaceDetails : IShareable
{
    public string Id { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public IReadOnlyCollection<FolderDetails> Folders { get; set; } = null!;
    public SharingDetails SharingDetails { get; set; } = null!;
}
