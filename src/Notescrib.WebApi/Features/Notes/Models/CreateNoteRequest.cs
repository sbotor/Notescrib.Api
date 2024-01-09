using Notescrib.Features.Notes.Commands;
using Notescrib.Models;

namespace Notescrib.WebApi.Features.Notes.Models;

public class CreateNoteRequest
{
    public string Name { get; set; } = null!;
    public Guid? FolderId { get; set; }
    public SharingInfo? SharingInfo { get; set; }
    public IReadOnlyCollection<string> Tags { get; set; } = Array.Empty<string>();
    public string? Content { get; set; }

    public CreateNote.Command ToCommand()
        => new(Name, FolderId, Content, Tags, SharingInfo ?? new());
}
