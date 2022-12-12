using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Folders.Models;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.Tree;

namespace Notescrib.Notes.Features.Folders.Commands;

public static class CreateFolder
{
    public record Command(string Name, string? ParentId) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(
            IWorkspaceRepository workspaceRepository,
            IFolderRepository folderRepository,
            IPermissionGuard permissionGuard,
            IDateTimeProvider dateTimeProvider)
        {
            _workspaceRepository = workspaceRepository;
            _folderRepository = folderRepository;
            _permissionGuard = permissionGuard;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _permissionGuard.UserContext.UserId;
            var now = _dateTimeProvider.Now;
            var folder = new Folder { Name = request.Name, Created = now, OwnerId = userId };

            var includeOptions = new FolderIncludeOptions() { Workspace = true };

            var parent = request.ParentId == null
                ? await _folderRepository.GetRootAsync(userId, includeOptions, cancellationToken)
                : await _folderRepository.GetByIdAsync(request.ParentId, includeOptions, cancellationToken);
            if (parent == null)
            {
                throw new NotFoundException<Folder>(request.ParentId);
            }

            _permissionGuard.GuardCanEdit(parent.OwnerId);
            
            var workspace = parent.Workspace;
            if (workspace.FolderCount >= Consts.Workspace.MaxFolderCount)
            {
                throw new AppException("Maximum folder count reached.");
            }

            if (parent.AncestorIds.Count >= Consts.Folder.MaxNestingLevel)
            {
                throw new AppException("The parent cannot nest children.");
            }

            folder.AncestorIds = parent.AncestorIds.Append(parent.Id).ToArray();

            folder.WorkspaceId = workspace.Id;
            workspace.FolderCount++;

            await _folderRepository.AddAsync(folder, cancellationToken);
            await _workspaceRepository.UpdateAsync(workspace, CancellationToken.None);

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

            RuleFor(x => x.ParentId)
                .NotEmpty();
        }
    }
}
