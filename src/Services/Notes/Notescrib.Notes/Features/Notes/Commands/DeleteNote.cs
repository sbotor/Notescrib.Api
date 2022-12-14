using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Commands;

public static class DeleteNote
{
    public record Command(string Id) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly INoteRepository _noteRepository;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(INoteRepository noteRepository, IWorkspaceRepository workspaceRepository,
            IPermissionGuard permissionGuard)
        {
            _noteRepository = noteRepository;
            _workspaceRepository = workspaceRepository;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var workspace = await _workspaceRepository
                .GetByOwnerIdAsync(_permissionGuard.UserContext.UserId, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException(ErrorCodes.Workspace.WorkspaceNotFound);
            }

            var include = new NoteIncludeOptions { Workspace = true };
            
            var note = await _noteRepository.GetByIdAsync(request.Id, include, cancellationToken);
            if (note == null)
            {
                throw new NotFoundException(ErrorCodes.Note.NoteNotFound);
            }

            _permissionGuard.GuardCanEdit(note.OwnerId);

            workspace.NoteCount--;
            
            await _noteRepository.DeleteAsync(request.Id, cancellationToken);
            await _workspaceRepository.UpdateAsync(workspace, CancellationToken.None);

            return Unit.Value;
;        }
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
