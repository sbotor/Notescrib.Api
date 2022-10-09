namespace Notescrib.Api.Application.Contracts.Workspace;

public class FolderRequest
{
    public string ParentPath { get; set; } = null!;
    public string Name { get; set; } = null!;
}
