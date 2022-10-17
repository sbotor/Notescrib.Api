using Notescrib.Api.Core.Contracts;

namespace Notescrib.Api.Core.Entities;

public class Folder : IWorkspacePath, IShareable
{
    public const string Separator = "/";

    public string Name { get; set; } = string.Empty;
    public string? ParentPath { get; set; }
    public string WorkspaceId { get; set; } = string.Empty;
    public SharingDetails SharingDetails { get; set; } = new();

    public string AbsolutePath => string.IsNullOrWhiteSpace(ParentPath)
        ? string.Join(Separator, WorkspaceId, Name)
        : string.Join(Separator, WorkspaceId, ParentPath, Name);

    public string[] Segments => AbsolutePath.Split(Separator);
    public bool IsRoot => string.IsNullOrWhiteSpace(ParentPath);
}
