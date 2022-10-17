using Notescrib.Api.Application.Notes.Models;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Models;

public class FolderDetails : IShareable, IWorkspacePath
{
    public string Name { get; set; } = null!;
    public string ParentPath { get; set; } = null!;
    public string WorkspaceId { get; set; } = null!;
    public bool IsRoot { get; set; }
    public IReadOnlyCollection<NoteOverview> Notes { get; set; } = null!;
    public SharingDetails SharingDetails { get; set; } = null!;
}
