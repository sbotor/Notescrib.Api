using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Data.MongoDb;
using Notescrib.Features.Folders;
using Notescrib.Utils;

namespace Notescrib.Features.Workspaces.Commands;

public static class CreateWorkspace
{
    public record Command : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IMongoDbContext _context;
        private readonly IUserContextProvider _userContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(
            IMongoDbContext context,
            IUserContextProvider userContext,
            IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _userContext = userContext;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            if (await _context.Workspaces.GetByOwnerIdAsync(cancellationToken) != null)
            {
                throw new DuplicationException(ErrorCodes.Workspace.WorkspaceAlreadyExists);
            }

            var now = _dateTimeProvider.Now;
            var workspace = new Workspace { OwnerId = _userContext.UserId, Created = now };

            await _context.EnsureTransactionAsync(CancellationToken.None);
            
            await _context.Workspaces.AddAsync(workspace, cancellationToken);

            await _context.Folders.CreateAsync(
                new()
                {
                    Id = workspace.Id,
                    OwnerId = workspace.OwnerId,
                    Name = Folder.RootName,
                    Created = workspace.Created,
                    WorkspaceId = workspace.Id
                },
                CancellationToken.None);

            await _context.CommitTransactionAsync();

            return Unit.Value;
        }
    }
}
