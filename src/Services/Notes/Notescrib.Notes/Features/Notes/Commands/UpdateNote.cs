using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Folders.Repositories;
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
            SharingInfo SharingInfo)
        : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(
            IFolderRepository folderRepository,
            IPermissionGuard permissionGuard,
            IDateTimeProvider dateTimeProvider)
        {
            _folderRepository = folderRepository;
            _permissionGuard = permissionGuard;
            _dateTimeProvider = dateTimeProvider;
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
            
            if (folder!.Children.Any(x => x.Name == request.Name))
            {
                throw new DuplicationException(ErrorCodes.Note.NoteAlreadyExists);
            }

            note.Name = request.Name;
            note.Tags = request.Tags.ToArray();
            note.SharingInfo = request.SharingInfo;
            note.Updated = _dateTimeProvider.Now;

            await _folderRepository.UpdateAsync(folder, cancellationToken);

            return Unit.Value;
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
        }
    }
}
