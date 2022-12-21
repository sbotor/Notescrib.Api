using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Templates.Repositories;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Workspaces.Commands;

public static class DeleteWorkspace
{
    public record Command : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly INoteRepository _noteRepository;
        private readonly INoteTemplateRepository _noteTemplateRepository;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(IWorkspaceRepository workspaceRepository, IFolderRepository folderRepository,
            INoteRepository noteRepository, INoteTemplateRepository noteTemplateRepository, IPermissionGuard permissionGuard)
        {
            _workspaceRepository = workspaceRepository;
            _folderRepository = folderRepository;
            _noteRepository = noteRepository;
            _noteTemplateRepository = noteTemplateRepository;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var workspace =
                await _workspaceRepository.GetByOwnerIdAsync(_permissionGuard.UserContext.UserId,
                    CancellationToken.None);
            if (workspace == null)
            {
                throw new NotFoundException(ErrorCodes.Workspace.WorkspaceNotFound);
            }

            await _noteTemplateRepository.DeleteAllAsync(workspace.Id, CancellationToken.None);
            await _noteRepository.DeleteAllAsync(workspace.Id, CancellationToken.None);
            await _folderRepository.DeleteAllAsync(workspace.Id, CancellationToken.None);

            await _workspaceRepository.DeleteAsync(workspace.Id, CancellationToken.None);

            return Unit.Value;
        }
    }
}
