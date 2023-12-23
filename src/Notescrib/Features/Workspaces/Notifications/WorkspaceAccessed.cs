using MediatR;
using Microsoft.EntityFrameworkCore;
using Notescrib.Core.Services;
using Notescrib.Data;
using Notescrib.Features.Folders;

namespace Notescrib.Features.Workspaces.Notifications;

public static class WorkspaceAccessed
{
    public record Notification(string UserId) : INotification;

    internal class Handler : INotificationHandler<Notification>
    {
        private readonly NotescribDbContext _dbContext;
        private readonly IClock _clock;

        public Handler(
            NotescribDbContext dbContext,
            IClock clock)
        {
            _dbContext = dbContext;
            _clock = clock;
        }
        
        public async Task Handle(Notification notification, CancellationToken cancellationToken)
        {
            var userId = notification.UserId;
            
            var workspace = await _dbContext.Workspaces.AsNoTracking()
                .FirstOrDefaultAsync(x => x.OwnerId == userId, CancellationToken.None);

            if (workspace is not null)
            {
                return;
            }
            
            var now = _clock.Now;
            
            workspace = new Workspace { OwnerId = userId, Created = now };
            _dbContext.Add(workspace);
            
            _dbContext.Add(new Folder
            {
                Id = workspace.Id,
                OwnerId = userId,
                Name = string.Empty,
                Created = now,
                WorkspaceId = workspace.Id
            });

            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }
    }
}
