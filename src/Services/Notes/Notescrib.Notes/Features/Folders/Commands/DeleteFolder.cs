using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Folders.Utils;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Folders.Commands;

public static class DeleteFolder
{
    public record Command(string Id) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly INoteRepository _noteRepository;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(IWorkspaceRepository workspaceRepository, INoteRepository noteRepository,
            IPermissionGuard permissionGuard)
        {
            _workspaceRepository = workspaceRepository;
            _noteRepository = noteRepository;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _permissionGuard.UserContext.UserId;
            var workspace = await _workspaceRepository.GetByOwnerIdAsync(userId, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>();
            }

            _permissionGuard.GuardCanEdit(workspace.OwnerId);

            var tree = new FolderTree(workspace);
            var foundFolder = tree.FindWithParent(x => x.Id == request.Id);
            if (foundFolder == null)
            {
                throw new NotFoundException<Folder>(request.Id);
            }

            var removedFolderIds = foundFolder.Item.EnumerateChildren().Select(x => x.Id).Append(foundFolder.Item.Id);
            await _noteRepository.DeleteFromFoldersAsync(removedFolderIds, cancellationToken);

            tree.Remove(foundFolder);

            await _workspaceRepository.UpdateAsync(workspace, cancellationToken);

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .NotEqual(Folder.RootId);
        }
    }
}
