using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Commands;

public static class UpdateNoteContent
{
    public record Command(string NoteId, string Content) : ICommand;

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
            var note = await _noteRepository.GetByIdAsync(request.NoteId, cancellationToken: cancellationToken);
            if (note == null)
            {
                throw new NotFoundException(ErrorCodes.Note.NoteNotFound);
            }
            
            _permissionGuard.GuardCanEdit(note.OwnerId);

            note.Content = request.Content;
            note.Updated = _dateTimeProvider.Now;
            
            await _noteRepository.UpdateContentAsync(note, CancellationToken.None);

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.NoteId)
                .NotEmpty();

            RuleFor(x => x.Content)
                .MaximumLength(Consts.Note.MaxContentLength);
        }
    }
}
