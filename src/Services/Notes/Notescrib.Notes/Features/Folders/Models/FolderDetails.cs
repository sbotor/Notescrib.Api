using Notescrib.Notes.Features.Notes.Models;

namespace Notescrib.Notes.Features.Folders.Models;

public class FolderDetails : FolderOverview
{
    public ICollection<FolderOverview> Children { get; set; } = Array.Empty<FolderOverview>();
    public IReadOnlyCollection<NoteOverview> Notes { get; set; } = Array.Empty<NoteOverview>();
}
