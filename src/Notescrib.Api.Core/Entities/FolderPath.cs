namespace Notescrib.Api.Core.Entities;

public class FolderPath
{
    private const string NullString = "NULL";

    public const string Separator = "/";

    public string AbsolutePath { get; private set; } = NullString;
    public string[] Segments { get; private set; } = Array.Empty<string>();
    public string WorkspaceId { get; private set; } = NullString;
    public bool IsRoot { get; private set; }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            Update();
        }
    }

    private string _parentPath = string.Empty;
    public string ParentPath
    {
        get => _parentPath;
        set
        {
            _parentPath = value;
            Update();
        }
    }

    private void Update()
    {
        AbsolutePath = $"{ParentPath}{Separator}{Name}";
        Segments = AbsolutePath.Split(Separator);
        WorkspaceId = Segments.FirstOrDefault() ?? NullString;
        IsRoot = Segments.ElementAtOrDefault(1) == Name;
    }
}
