using Notescrib.Api.Application.Notes.Commands;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Contracts.Notes;

public class AddNoteRequest
{
    public string Name { get; set; } = null!;
    public string WorkspaceId { get; set; } = null!;
    public string? ParentPath { get; set; }
    public SharingDetails SharingDetails { get; set; } = new();

    public AddNote.Command ToCommand()
        => new(Name, WorkspaceId, ParentPath, SharingDetails);
}
