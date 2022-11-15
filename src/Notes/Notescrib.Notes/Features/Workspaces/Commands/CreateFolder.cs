using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Features.Workspaces.Utils;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Workspaces.Commands;

public static class CreateFolder
{
    public record Command(string WorkspaceId, string Name, string? Parent) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IWorkspaceRepository _repository;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(IWorkspaceRepository repository, IPermissionGuard permissionGuard)
        {
            _repository = repository;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var workspace = await _repository.GetWorkspaceByIdAsync(request.WorkspaceId, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>(request.WorkspaceId);
            }
            
            _permissionGuard.GuardCanEdit(workspace.OwnerId);

            var folder = new Folder { Name = request.Name };
            var tree = new FolderTree(workspace.Folders);
            
            tree.Add(folder, request.Parent);
            workspace.Folders = tree.Roots.ToList();

            await _repository.UpdateWorkspaceAsync(workspace, cancellationToken);

            return Unit.Value;
        }
    }
}
