using Notescrib.Notes.Models;

namespace Notescrib.Notes.Features.Notes.Models;

public class NoteOverview
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string FolderId { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = null!;
    public IReadOnlyCollection<string> Tags { get; set; } = null!;
    public bool IsReadonly { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}
