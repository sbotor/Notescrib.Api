using Notescrib.Api.Application.Notes.Models;

namespace Notescrib.Api.Application.Workspaces.Models;

public class FolderDetails : FolderOverview
{
    public IReadOnlyCollection<NoteOverview> Notes { get; set; } = null!;
}
