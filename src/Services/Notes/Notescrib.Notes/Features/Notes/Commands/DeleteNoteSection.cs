using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils.Tree;

namespace Notescrib.Notes.Features.Notes.Commands;

public static class DeleteNoteSection
{
    public record Command(string NoteId, string SectionId) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly INoteRepository _noteRepository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(INoteRepository noteRepository, IPermissionGuard permissionGuard, IDateTimeProvider dateTimeProvider)
        {
            _noteRepository = noteRepository;
            _permissionGuard = permissionGuard;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.GetByIdAsync(request.NoteId, cancellationToken);
            if (note == null)
            {
                throw new NotFoundException<Note>(request.NoteId);
            }

            _permissionGuard.GuardCanEdit(note.OwnerId);

            var tree = new Tree<NoteSection>(note.SectionTree);
            var found = tree.VisitDepthFirst(x =>
            {
                if (x.Item.Id != request.SectionId)
                {
                    return false;
                }

                x.Parent!.Item.Children.Remove(x.Item);
                note.SectionCount--;

                return true;
            });

            if (!found)
            {
                throw new NotFoundException<NoteSection>(request.SectionId);
            }

            await _noteRepository.UpdateAsync(note, cancellationToken);
            
            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.NoteId)
                .NotEmpty();
            
            RuleFor(x => x.SectionId)
                .NotEmpty()
                .NotEqual(NoteSection.RootId);
        }
    }
}
