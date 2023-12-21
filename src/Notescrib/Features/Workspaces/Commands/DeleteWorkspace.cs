using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Data.MongoDb;
using Notescrib.Utils;

namespace Notescrib.Features.Workspaces.Commands;

public static class DeleteWorkspace
{
    public record Command : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IMongoDbContext _context;

        public Handler(IMongoDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var workspace = await _context.Workspaces.GetByOwnerIdAsync(CancellationToken.None);
            if (workspace == null)
            {
                throw new NotFoundException(ErrorCodes.Workspace.WorkspaceNotFound);
            }

            await _context.EnsureTransactionAsync(CancellationToken.None);
            
            await _context.NoteTemplates.DeleteAllAsync(CancellationToken.None);
            await _context.Notes.DeleteAllAsync(CancellationToken.None);
            await _context.Folders.DeleteAllAsync(CancellationToken.None);
            await _context.Workspaces.DeleteAsync(workspace.Id, CancellationToken.None);

            await _context.CommitTransactionAsync();

            return Unit.Value;
        }
    }
}
