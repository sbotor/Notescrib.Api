using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes.Features.Workspaces.Commands;

public static class CreateWorkspace
{
    public record Command : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly MongoDbContext _context;
        private readonly IUserContextProvider _userContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(
            MongoDbContext context,
            IUserContextProvider userContext,
            IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _userContext = userContext;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _userContext.UserId;
            if (await _context.Workspaces.GetByOwnerIdAsync(userId, cancellationToken) != null)
            {
                throw new DuplicationException(ErrorCodes.Workspace.WorkspaceAlreadyExists);
            }

            var now = _dateTimeProvider.Now;
            var workspace = new Workspace { OwnerId = userId, Created = now };

            await _context.EnsureTransactionAsync(CancellationToken.None);
            
            await _context.Workspaces.AddAsync(workspace, cancellationToken);

            await _context.Folders.AddAsync(
                new()
                {
                    Id = workspace.Id,
                    OwnerId = workspace.OwnerId,
                    Name = "*root",
                    Created = workspace.Created,
                    WorkspaceId = workspace.Id
                },
                CancellationToken.None);

            await _context.CommitTransactionAsync();

            return Unit.Value;
        }
    }
}
