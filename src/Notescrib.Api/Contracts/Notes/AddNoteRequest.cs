using Notescrib.Api.Application.Notes.Commands;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Contracts.Notes;

public class AddNoteRequest
{
    public string Name { get; set; } = null!;
    public string FolderId { get; set; } = null!;
    public SharingInfo SharingInfo { get; set; } = new();

    public AddNote.Command ToCommand()
        => new(Name, FolderId, SharingInfo);
}
