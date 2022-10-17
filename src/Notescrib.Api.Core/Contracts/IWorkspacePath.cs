namespace Notescrib.Api.Core.Contracts;

public interface IWorkspacePath
{
    public string? ParentPath { get; }
    public string WorkspaceId { get; }
}
