using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Commands;

public static class UpdateNote
{
    public record Command(
            string Id,
            string Name,
            IReadOnlyCollection<string> Tags,
            IReadOnlyCollection<string> RelatedIds,
            SharingInfo SharingInfo)
        : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IFolderRepository _folderRepository;
        private readonly INoteRepository _noteRepository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(
            IFolderRepository folderRepository,
            INoteRepository noteRepository,
            IPermissionGuard permissionGuard,
            IDateTimeProvider dateTimeProvider)
        {
            _folderRepository = folderRepository;
            _noteRepository = noteRepository;
            _permissionGuard = permissionGuard;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
            if (note == null)
            {
                throw new NotFoundException(ErrorCodes.Note.NoteNotFound);
            }

            _permissionGuard.GuardCanEdit(note.OwnerId);

            var folder = await _folderRepository.GetByIdAsync(note.FolderId, cancellationToken: cancellationToken);
            var notes = await _noteRepository.GetByFolderIdAsync(folder!.Id, cancellationToken: cancellationToken);

            if (note.Name != request.Name && notes.Any(x => x.Name == request.Name))
            {
                throw new DuplicationException(ErrorCodes.Note.NoteAlreadyExists);
            }

            await ResolveRelatedNoteIds(request, note);

            note.Name = request.Name;
            note.Tags = request.Tags.ToArray();
            note.SharingInfo = request.SharingInfo;
            note.Updated = _dateTimeProvider.Now;

            await _noteRepository.UpdateAsync(note, cancellationToken);

            return Unit.Value;
        }

        private async Task ResolveRelatedNoteIds(Command command, NoteBase note)
        {
            var foundNotes = await _noteRepository.GetManyAsync(command.RelatedIds);

            if (foundNotes.Any(x => _permissionGuard.CanEdit(x.OwnerId)))
            {
                throw new ForbiddenException();
            }

            if (foundNotes.Count != command.RelatedIds.Count)
            {
                throw new NotFoundException(ErrorCodes.Note.NoteNotFound);
            }

            note.RelatedIds = command.RelatedIds.ToArray();
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Consts.Name.MaxLength);

            RuleFor(x => x.Tags.Count)
                .LessThanOrEqualTo(Consts.Note.MaxLabelCount);
            RuleForEach(x => x.Tags)
                .NotEmpty();

            RuleFor(x => x.SharingInfo)
                .NotNull();

            RuleFor(x => x.RelatedIds.Count)
                .LessThanOrEqualTo(Consts.Note.MaxRelatedCount);
        }
    }
}
