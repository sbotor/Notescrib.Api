using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils.Tree;

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
            var workspace = await _workspaceRepository.GetByOwnerIdAsync(
                _permissionGuard.UserContext.UserId,
                cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>();
            }

            _permissionGuard.GuardCanEdit(workspace.OwnerId);

            var tree = new Tree<Folder>(workspace.FolderTree);
            var found = await tree.VisitDepthFirstAsync(async x =>
            {
                if (x.Item.Id != request.Id)
                {
                    return false;
                }

                var removedFolderIds = x.Item.ToDfsEnumerable()
                    .Select(n => n.Item.Id).ToArray();
                
                await _noteRepository.DeleteFromFoldersAsync(removedFolderIds, cancellationToken);
                x.Parent!.Item.ChildrenIds.Remove(x.Item);
                workspace.FolderCount -= removedFolderIds.Length;
                
                return true;
            });
            
            if (!found)
            {
                throw new NotFoundException<Folder>(request.Id);
            }

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
