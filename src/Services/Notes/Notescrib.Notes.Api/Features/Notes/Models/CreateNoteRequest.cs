using Notescrib.Notes.Features.Notes.Commands;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Models;

namespace Notescrib.Notes.Api.Features.Notes.Models;

public class CreateNoteRequest
{
    public string Name { get; set; } = null!;
    public string? FolderId { get; set; }
    public SharingInfo? SharingInfo { get; set; }
    public IReadOnlyCollection<string> Labels { get; set; } = Array.Empty<string>();

    public CreateNote.Command ToCommand()
        => new(Name, FolderId ?? Folder.RootId, Labels, SharingInfo ?? new());
}
