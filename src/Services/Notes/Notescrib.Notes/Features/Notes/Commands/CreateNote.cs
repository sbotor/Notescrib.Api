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

public static class CreateNote
{
    public record Command(
            string Name,
            string? FolderId,
            IReadOnlyCollection<string> Tags,
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
            var userId = _permissionGuard.UserContext.UserId;
            
            var folder = request.FolderId == null
                ? await _folderRepository.GetRootAsync(userId, cancellationToken: cancellationToken)
                : await _folderRepository.GetByIdAsync(request.FolderId, cancellationToken: cancellationToken);
            if (folder == null)
            {
                throw new NotFoundException(ErrorCodes.Folder.FolderNotFound);
            }
            
            _permissionGuard.GuardCanEdit(folder.OwnerId);

            var notes = await _noteRepository.GetByFolderIdAsync(folder.Id, cancellationToken: cancellationToken);
            
            if (notes.Count >= Consts.Folder.MaxNoteCount)
            {
                throw new AppException(ErrorCodes.Folder.MaximumNoteCountReached);
            }
            
            if (notes.Any(x => x.Name == request.Name))
            {
                throw new AppException(ErrorCodes.Note.NoteAlreadyExists);
            }

            var now = _dateTimeProvider.Now;
            var note = new NoteData
            {
                Name = request.Name,
                Tags = request.Tags.ToArray(),
                OwnerId = userId,
                SharingInfo = request.SharingInfo,
                Created = now,
                FolderId = folder.Id,
                WorkspaceId = folder.WorkspaceId
            };
            
            folder.Updated = now;
            
            await _noteRepository.CreateAsync(note, cancellationToken);
            await _folderRepository.UpdateAsync(folder, CancellationToken.None);
            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
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
