namespace Notescrib.Api.Core.Entities;

public class Note : EntityIdBase
{
    public string Name { get; set; } = string.Empty;
    public string FullParentPath { get; set; } = string.Empty;
    public NoteSection? RootSection { get; set; }
    public ICollection<string> Labels { get; set; } = new List<string>();
    public string UserId { get; set; } = string.Empty;
}
