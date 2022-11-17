using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Workspaces.Commands;

public static class DeleteWorkspace
{
    public record Command(string Id) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly INoteRepository _noteRepository;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(IWorkspaceRepository workspaceRepository, INoteRepository noteRepository, IPermissionGuard permissionGuard)
        {
            _workspaceRepository = workspaceRepository;
            _noteRepository = noteRepository;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(request.Id, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>();
            }
            
            _permissionGuard.GuardCanEdit(workspace.OwnerId);

            await _noteRepository.DeleteNotesFromWorkspaceAsync(workspace.Id, null, cancellationToken);
            await _workspaceRepository.DeleteAsync(workspace.Id, cancellationToken);
            
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
