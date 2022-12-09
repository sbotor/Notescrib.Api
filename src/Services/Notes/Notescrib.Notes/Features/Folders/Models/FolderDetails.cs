using Notescrib.Notes.Features.Notes.Models;

namespace Notescrib.Notes.Features.Folders.Models;

public class FolderDetails : FolderInfoBase
{
    public ICollection<FolderInfoBase> Children { get; set; } = Array.Empty<FolderInfoBase>();
    public IReadOnlyCollection<NoteOverview> Notes { get; set; } = Array.Empty<NoteOverview>();
}
