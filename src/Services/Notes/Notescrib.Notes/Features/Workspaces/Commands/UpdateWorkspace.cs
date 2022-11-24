using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Workspaces.Commands;

public static class UpdateWorkspace
{
    public record Command(string Id, string Name) : ICommand;

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
            var workspace = await _repository.GetWorkspaceByIdAsync(request.Id, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>();
            }
            
            _permissionGuard.GuardCanEdit(workspace.OwnerId);

            if (workspace.Name != request.Name
                && await _repository.ExistsAsync(workspace.OwnerId, request.Name, cancellationToken))
            {
                throw new DuplicationException<Workspace>();
            }
            
            workspace.Name = request.Name;

            await _repository.UpdateWorkspaceAsync(workspace, cancellationToken);
            
            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Size.Name.MaxLength);
        }
    }
}
