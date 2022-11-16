namespace Notescrib.Notes.Models.Configuration;

public class MongoDbSettings
{
    public string ConnectionUri { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public CollectionNames Collections { get; set; } = new();
}

public class CollectionNames
{
    public string Workspaces { get; set; } = nameof(Workspaces);
    public string Notes { get; set; } = nameof(Notes);
}
