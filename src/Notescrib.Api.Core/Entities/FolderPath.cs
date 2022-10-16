namespace Notescrib.Api.Core.Entities;

public class FolderPath
{
    public const string Separator = "/";

    public string Name { get; set; } = string.Empty;
    public string ParentPath { get; set; } = string.Empty;
    public string WorkspaceId { get; set; } = string.Empty;

    public string AbsolutePath => string.Join(Separator, WorkspaceId, ParentPath, Name);
    public string[] Segments => AbsolutePath.Split(Separator);
    public bool IsRoot => string.IsNullOrWhiteSpace(ParentPath);
}
