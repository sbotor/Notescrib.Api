// using Notescrib.Notes.Features.Notes;
// using Notescrib.Notes.Features.Notes.Commands;
//
// namespace Notescrib.Notes.Api.Features.Notes.Models;
//
// public class UpdateNoteContentsRequest
// {
//     public string NoteId { get; set; } = null!;
//     public string? ParentId { get; set; }
//     public string Name { get; set; } = null!;
//     public string Content { get; set; } = null!;
//
//     public UpdateNoteSection.Command ToCommand(string noteId, string sectionId)
//         => new(noteId, sectionId, ParentId ?? NoteSection.RootId, Name, Content);
// }
