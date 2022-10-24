using Notescrib.Api.Application.Notes.Models;

namespace Notescrib.Api.Application.Workspaces.Models;

public class FolderDetails : FolderInfoBase
{
    public IReadOnlyCollection<NoteOverview> Notes { get; set; } = null!;
}
