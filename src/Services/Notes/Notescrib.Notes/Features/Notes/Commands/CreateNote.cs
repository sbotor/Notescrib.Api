using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
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
        private readonly INoteRepository _noteRepository;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(
            INoteRepository noteRepository,
            IWorkspaceRepository workspaceRepository,
            IFolderRepository folderRepository,
            IPermissionGuard permissionGuard,
            IDateTimeProvider dateTimeProvider)
        {
            _noteRepository = noteRepository;
            _workspaceRepository = workspaceRepository;
            _folderRepository = folderRepository;
            _permissionGuard = permissionGuard;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _permissionGuard.UserContext.UserId;

            var includeOptions = new FolderIncludeOptions { Workspace = true, Notes = true };

            var folder = request.FolderId == null
                ? await _folderRepository.GetRootAsync(userId, includeOptions, cancellationToken)
                : await _folderRepository.GetByIdAsync(request.FolderId, includeOptions, cancellationToken);
            if (folder == null)
            {
                throw new NotFoundException(ErrorCodes.Folder.FolderNotFound);
            }
            
            _permissionGuard.GuardCanEdit(folder.OwnerId);

            if (folder.Children.Any(x => x.Name == request.Name))
            {
                throw new AppException(ErrorCodes.Note.NoteAlreadyExists);
            }
            
            var workspace = folder.Workspace;
            if (workspace.NoteCount >= Consts.Workspace.MaxNoteCount)
            {
                throw new AppException("Maximum note count reached.");
            }
            
            var note = new NoteBase
            {
                Name = request.Name,
                Tags = request.Tags.ToArray(),
                OwnerId = userId,
                SharingInfo = request.SharingInfo,
                Created = _dateTimeProvider.Now,
                FolderId = folder.Id,
                WorkspaceId = folder.WorkspaceId
            };

            workspace.NoteCount++;
            
            await _noteRepository.CreateNote(note, cancellationToken);
            await _workspaceRepository.UpdateAsync(workspace, CancellationToken.None);

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FolderId)
                .NotNull();

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
