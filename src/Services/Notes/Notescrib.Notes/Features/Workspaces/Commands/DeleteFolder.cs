using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Features.Workspaces.Utils;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Workspaces.Commands;

public static class DeleteFolder
{
    public record Command(string WorkspaceId, string Id) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly INoteRepository _noteRepository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(IWorkspaceRepository workspaceRepository, INoteRepository noteRepository,
            IPermissionGuard permissionGuard, IDateTimeProvider dateTimeProvider)
        {
            _workspaceRepository = workspaceRepository;
            _noteRepository = noteRepository;
            _permissionGuard = permissionGuard;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(request.WorkspaceId, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>(request.WorkspaceId);
            }

            _permissionGuard.GuardCanEdit(workspace.OwnerId);

            var tree = new FolderTree(workspace);
            var foundFolder = tree.FindWithParent(x => x.Id == request.Id);
            if (foundFolder == null)
            {
                throw new NotFoundException<Folder>(request.Id);
            }

            var removedFolderIds = foundFolder.Item.EnumerateChildren().Select(x => x.Id).Append(foundFolder.Item.Id);
            await _noteRepository.DeleteNotesFromWorkspaceAsync(workspace.Id, removedFolderIds, cancellationToken);

            tree.Remove(foundFolder);
            workspace.Updated = _dateTimeProvider.Now;

            await _workspaceRepository.UpdateWorkspaceAsync(workspace, cancellationToken);

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.WorkspaceId)
                .NotEmpty();

            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}
