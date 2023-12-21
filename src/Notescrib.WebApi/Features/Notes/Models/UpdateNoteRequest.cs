using Notescrib.Features.Notes.Commands;
using Notescrib.Models;

namespace Notescrib.WebApi.Features.Notes.Models;

public class UpdateNoteRequest
{
    public string Name { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = null!;
    public IReadOnlyCollection<string> Tags { get; set; } = null!;

    public UpdateNote.Command ToCommand(string id)
        => new(id, Name, Tags, SharingInfo);
}
