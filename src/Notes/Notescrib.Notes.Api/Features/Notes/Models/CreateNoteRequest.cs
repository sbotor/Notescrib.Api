using Notescrib.Notes.Features.Notes.Commands;
using Notescrib.Notes.Models;

namespace Notescrib.Notes.Api.Features.Notes.Models;

public class CreateNoteRequest
{
    public string Name { get; set; } = null!;
    public string WorkspaceId { get; set; } = null!;
    public string? Folder { get; set; }
    public SharingInfo? SharingInfo { get; set; }

    public CreateNote.Command ToCommand()
        => new(Name, WorkspaceId, Folder, SharingInfo);
}
