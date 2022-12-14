using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.Tree;

namespace Notescrib.Notes.Features.Folders.Commands;

public static class DeleteFolder
{
    public record Command(string Id) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly INoteRepository _noteRepository;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(IFolderRepository folderRepository, IWorkspaceRepository workspaceRepository,
            INoteRepository noteRepository, IPermissionGuard permissionGuard)
        {
            _workspaceRepository = workspaceRepository;
            _folderRepository = folderRepository;
            _noteRepository = noteRepository;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = await _folderRepository.GetByIdAsync(
                request.Id,
                new() { Workspace = true, Children = true},
                cancellationToken);
            if (folder == null)
            {
                throw new NotFoundException(ErrorCodes.Folder.FolderNotFound);
            }
            
            _permissionGuard.GuardCanEdit(folder.OwnerId);

            if (folder.ParentId == null)
            {
                throw new AppException("Cannot delete root folder.");
            }

            var workspace = folder.Workspace;

            var folderIds = folder.Children.Select(x => x.Id)
                .Append(folder.Id)
                .ToArray();
            
            var deletedNoteCount = await _noteRepository.DeleteFromFoldersAsync(folderIds, CancellationToken.None);

            await _folderRepository.DeleteManyAsync(folderIds, CancellationToken.None);

            await _workspaceRepository.UpdateAsync(workspace, CancellationToken.None);
            
            workspace.NoteCount -= deletedNoteCount;

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
