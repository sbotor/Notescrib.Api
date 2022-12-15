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
        private readonly INoteContentRepository _noteContentRepository;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(IFolderRepository folderRepository, INoteContentRepository noteContentRepository, IPermissionGuard permissionGuard)
        {
            _folderRepository = folderRepository;
            _noteContentRepository = noteContentRepository;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = await _folderRepository.GetByNoteIdAsync(request.Id, cancellationToken: cancellationToken);
            if (folder == null)
            {
                throw new NotFoundException(ErrorCodes.Note.NoteNotFound);
            }

            var note = folder.FindNote(request.Id);

            _permissionGuard.GuardCanEdit(note.OwnerId);

            folder.Notes.Remove(note);
            await _folderRepository.UpdateAsync(folder, cancellationToken);
            await _noteContentRepository.DeleteAsync(note.Id, CancellationToken.None);
            
            return Unit.Value;
;        }
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
