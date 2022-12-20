using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Commands;

public static class DeleteNote
{
    public record Command(string Id) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IFolderRepository _folderRepository;
        private readonly INoteRepository _noteRepository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(IFolderRepository folderRepository, INoteRepository noteRepository,
            IPermissionGuard permissionGuard, IDateTimeProvider dateTimeProvider)
        {
            _folderRepository = folderRepository;
            _noteRepository = noteRepository;
            _permissionGuard = permissionGuard;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.GetByIdAsync(request.Id, new() { Folder = true }, cancellationToken);
            if (note == null)
            {
                throw new NotFoundException(ErrorCodes.Note.NoteNotFound);
            }

            _permissionGuard.GuardCanEdit(note.OwnerId);

            note.Folder.Updated = _dateTimeProvider.Now;

            await _folderRepository.UpdateAsync(note.Folder, cancellationToken);
            await _noteRepository.DeleteAsync(note, CancellationToken.None);

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}
