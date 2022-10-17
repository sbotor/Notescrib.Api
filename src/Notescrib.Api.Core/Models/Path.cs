using Notescrib.Api.Core.Contracts;

namespace Notescrib.Api.Core.Models;

public class WorkspacePath : IWorkspacePath
{
    public string? ParentPath { get; }
    public string WorkspaceId { get; } = string.Empty;
}
