using Notescrib.Notes.Application.Notes.Models;

namespace Notescrib.Notes.Application.Workspaces.Models;

public class FolderDetails : FolderOverview
{
    public IReadOnlyCollection<NoteOverview> Notes { get; set; } = null!;
}
