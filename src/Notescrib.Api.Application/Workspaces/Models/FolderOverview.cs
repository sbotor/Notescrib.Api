using Notescrib.Api.Application.Notes.Models;
using Notescrib.Api.Core.Contracts;

namespace Notescrib.Api.Application.Workspaces.Models;

public class FolderOverview : FolderDetails
{
    public IPagedList<NoteOverview> Notes { get; set; } = null!;
    public string WorkspaceId { get; set; } = null!;
}
