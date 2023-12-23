using MediatR;
using Microsoft.EntityFrameworkCore;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Data;
using Notescrib.Utils;

namespace Notescrib.Features.Workspaces.Commands;

public static class DeleteWorkspace
{
    public record Command : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly NotescribDbContext _dbContext;
        private readonly IUserContext _userContext;

        public Handler(NotescribDbContext dbContext, IUserContext userContext)
        {
            _dbContext = dbContext;
            _userContext = userContext;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = await _userContext.GetUserId(CancellationToken.None);
            var workspace = await _dbContext.Workspaces
                .FirstOrDefaultAsync(x => x.OwnerId == userId, CancellationToken.None)
                ?? throw new NotFoundException(ErrorCodes.Workspace.WorkspaceNotFound);

            _dbContext.Remove(workspace);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            return Unit.Value;
        }
    }
}
