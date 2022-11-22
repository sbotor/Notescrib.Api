﻿using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Features.Workspaces.Utils;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Workspaces.Commands;

public static class UpdateFolder
{
    public record Command(string WorkspaceId, string FolderId, string Name, string? ParentId) : ICommand;

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
                throw new NotFoundException<Workspace>();
            }
            
            _permissionGuard.GuardCanEdit(request.WorkspaceId);

            var tree = new FolderTree(workspace.Folders);
            var found = tree.FindWithParent(x => x.Id == request.FolderId);
            if (found == null)
            {
                throw new NotFoundException<Folder>();
            }

            found.Item.Name = request.Name;
            tree.Move(found, request.ParentId);

            await _repository.UpdateWorkspaceAsync(workspace, cancellationToken);
            
            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FolderId)
                .NotEmpty();
            
            RuleFor(x => x.WorkspaceId)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Size.Name.MaxLength);

            RuleFor(x => x.ParentId)
                .NotEmpty()
                .When(x => x.ParentId != null);
        }
    }
}
