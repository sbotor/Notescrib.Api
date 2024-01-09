using Notescrib.Models;

namespace Notescrib.Features.Notes.Models;

public class NoteOverview
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid FolderId { get; set; }
    public string OwnerId { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = null!;
    public IReadOnlyCollection<string> Tags { get; set; } = null!;
    public bool IsReadonly { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
}
