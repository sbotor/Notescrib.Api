namespace Notescrib.Notes.Models.Configuration;

public class MongoDbSettings
{
    public string ConnectionUri { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public MongoDbCollectionNames Collections { get; set; } = new();
}

public class MongoDbCollectionNames
{
    public string Workspaces { get; set; } = nameof(Workspaces);
    public string Folders { get; set; } = nameof(Folders);
    public string Notes { get; set; } = nameof(Notes);
    public string NoteContents { get; set; } = nameof(NoteContents);
    public string NoteTemplates { get; set; } = nameof(NoteTemplates);
}
