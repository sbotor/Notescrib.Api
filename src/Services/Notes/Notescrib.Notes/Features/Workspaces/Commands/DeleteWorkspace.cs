using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Templates.Repositories;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes.Features.Workspaces.Commands;

public static class DeleteWorkspace
{
    public record Command : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly MongoDbContext _context;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(MongoDbContext context, IPermissionGuard permissionGuard)
        {
            _context = context;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var workspace =
                await _context.Workspaces.GetByOwnerIdAsync(_permissionGuard.UserContext.UserId,
                    CancellationToken.None);
            if (workspace == null)
            {
                throw new NotFoundException(ErrorCodes.Workspace.WorkspaceNotFound);
            }

            await _context.EnsureTransactionAsync(CancellationToken.None);
            
            await _context.NoteTemplates.DeleteAllAsync(workspace.Id, CancellationToken.None);
            await _context.Notes.DeleteAllAsync(workspace.Id, CancellationToken.None);
            await _context.Folders.DeleteAllAsync(workspace.Id, CancellationToken.None);
            await _context.Workspaces.DeleteAsync(workspace.Id, CancellationToken.None);

            await _context.CommitTransactionAsync();

            return Unit.Value;
        }
    }
}
