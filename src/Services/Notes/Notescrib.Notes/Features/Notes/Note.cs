using Notescrib.Notes.Contracts;
using Notescrib.Notes.Models;

namespace Notescrib.Notes.Features.Notes;

public class Note : IShareable
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string FolderId { get; set; } = null!;
    public NoteSection SectionTree { get; set; } = null!;
    public ICollection<string> Tags { get; set; } = new List<string>();
    public string OwnerId { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = null!;
    public int SectionCount { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}
