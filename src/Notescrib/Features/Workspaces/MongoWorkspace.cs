namespace Notescrib.Features.Workspaces;

public class MongoWorkspace
{
    public string Id { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public DateTime Created { get; set; }
}

