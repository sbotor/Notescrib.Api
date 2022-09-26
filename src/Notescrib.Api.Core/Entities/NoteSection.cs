namespace Notescrib.Api.Core.Entities;

public class NoteSection
{
    public string Name { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<NoteSection> Children { get; set; } = new();
}
