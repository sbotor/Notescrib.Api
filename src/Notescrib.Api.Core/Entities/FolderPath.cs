namespace Notescrib.Api.Core.Entities;

public class FolderPath
{
    private const string NullString = "NULL";

    public const string Separator = "/";

    public string Name { get; set; } = string.Empty;
    public string ParentPath { get; set; } = string.Empty;

    public string AbsolutePath => $"{ParentPath}{Separator}{Name}";
    public string[] Segments => AbsolutePath.Split(Separator);
    public string WorkspaceId => Segments.FirstOrDefault() ?? NullString;
    public bool IsRoot => Segments.ElementAtOrDefault(1) == Name;
}
