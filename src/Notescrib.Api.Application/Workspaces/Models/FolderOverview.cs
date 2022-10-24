namespace Notescrib.Api.Application.Workspaces.Models;

public class FolderOverview : FolderInfoBase
{
    public IReadOnlyCollection<FolderOverview> Children { get; set; } = null!;
}
