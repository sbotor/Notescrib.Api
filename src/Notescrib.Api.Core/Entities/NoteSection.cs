namespace Notescrib.Api.Core.Entities;

public class NoteSection
{
    public string Name { get; set; } = null!;
    public string Content { get; set; } = null!;
    public ICollection<NoteSection> Children { get; set; } = null!;
}
