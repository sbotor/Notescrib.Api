using MediatR;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Identity.Clients;
using Notescrib.Identity.Utils;

namespace Notescrib.Identity.Features.Users.Notifications;

public static class CreateWorkspace
{
    public record Notification(string Jwt) : INotification;

    public class Handler : INotificationHandler<Notification>
    {
        private readonly INotesApiClient _client;

        public Handler(INotesApiClient client)
        {
            _client = client;
        }

        public async Task Handle(Notification notification, CancellationToken cancellationToken)
        {
            var result = await _client.CreateWorkspaceAsync(notification.Jwt);
            if (!result)
            {
                throw new ServerErrorException(ErrorCodes.Notes.NotesError);
            }
        }
    }
}
