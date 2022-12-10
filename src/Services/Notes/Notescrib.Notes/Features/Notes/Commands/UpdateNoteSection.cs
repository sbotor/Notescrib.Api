// using FluentValidation;
// using MediatR;
// using Notescrib.Core.Cqrs;
// using Notescrib.Core.Models.Exceptions;
// using Notescrib.Notes.Extensions;
// using Notescrib.Notes.Features.Notes.Models;
// using Notescrib.Notes.Features.Notes.Repositories;
// using Notescrib.Notes.Features.Notes.Utils;
// using Notescrib.Notes.Services;
//
// namespace Notescrib.Notes.Features.Notes.Commands;
//
// public static class UpdateNoteSection
// {
//     public record Command(string NoteId, string SectionId, string ParentId, string Name, string Content) : ICommand;
//
//     internal class Handler : ICommandHandler<Command>
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
//         public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
//         {
//             var note = await _repository.GetByIdAsync(request.NoteId, cancellationToken);
//             if (note == null)
//             {
//                 throw new NotFoundException<Note>(request.NoteId);
//             }
//
//             _permissionGuard.GuardCanEdit(note.OwnerId);
//
//             var tree = MapContents(request.Sections);
//
//             note.NoteSectionTree = tree.Root.Children.ToList();
//
//             await _repository.UpdateAsync(note, cancellationToken);
//             return Unit.Value;
//         }
//
//         private static NoteSectionTree MapContents(IEnumerable<NoteContentsSection> original)
//             => new(original
//                 .MapTree(x => new NoteSection
//                 {
//                     Name = x.Name, Content = x.Content ?? string.Empty, Children = new List<NoteSection>()
//                 }).ToList());
//     }
//
//     internal class Validator : AbstractValidator<Command>
//     {
//         public Validator()
//         {
//             RuleFor(x => x.Id)
//                 .NotEmpty();
//
//             RuleForEach(x => x.Sections)
//                 .SetValidator(new NoteContentsSectionValidator());
//         }
//     }
// }
