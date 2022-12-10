// using Notescrib.Core.Cqrs;
// using Notescrib.Core.Models.Exceptions;
// using Notescrib.Notes.Features.Notes.Repositories;
// using Notescrib.Notes.Services;
//
// namespace Notescrib.Notes.Features.Notes.Commands;
//
// public static class CreateNoteSection
// {
//     public record Command(string NoteId, string ParentId, string Name) : ICommand<string>;
//
//     internal class Handler : ICommandHandler<Command, string>
//     {
//         private readonly INoteRepository _repository;
//         private readonly IPermissionGuard _permissionGuard;
//
//         public Handler(INoteRepository repository, IPermissionGuard permissionGuard)
//         {
//             _repository = repository;
//             _permissionGuard = permissionGuard;
//         }
//
//         public async Task<string> Handle(Command request, CancellationToken cancellationToken)
//         {
//             var note = await _repository.GetByIdAsync(request.NoteId, cancellationToken);
//             if (note == null)
//             {
//                 throw new NotFoundException<Note>(request.NoteId);
//             }
//             
//             _permissionGuard.GuardCanEdit(note.OwnerId);
//             
//             var 
//         }
//     }
// }
