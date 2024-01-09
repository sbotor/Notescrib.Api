using MediatR;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Identity.Clients;
using Notescrib.Identity.Utils;

namespace Notescrib.Identity.Features.Users.Notifications;

public static class DeleteWorkspace
{
    public record Notification(string Jwt) : INotification;

    internal class Handler : INotificationHandler<Notification>
    {
        private readonly INotesApiClient _client;

        public Handler(INotesApiClient client)
        {
            _client = client;
        }

        public async Task Handle(Notification notification, CancellationToken cancellationToken)
        {
            var result = await _client.DeleteWorkspaceAsync(notification.Jwt);
            if (!result)
            {
                throw new ServerErrorException(ErrorCodes.Notes.NotesError);
            }
        }
    }
}
