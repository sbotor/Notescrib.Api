using MediatR;
using Microsoft.AspNetCore.Http;
using Notescrib.Core.Services;
using Notescrib.Features.Workspaces.Notifications;

namespace Notescrib.Services;

public class NotesUserContext : UserContext
{
    private readonly IPublisher _publisher;

    public NotesUserContext(
        IPublisher publisher,
        IHttpContextAccessor httpContextAccessor)
        : base(httpContextAccessor)
    {
        _publisher = publisher;
    }

    protected override async ValueTask AfterUserExtraction(UserInfo user, CancellationToken cancellationToken)
        => await _publisher.Publish(new WorkspaceAccessed.Notification(user.UserId), cancellationToken);
}
