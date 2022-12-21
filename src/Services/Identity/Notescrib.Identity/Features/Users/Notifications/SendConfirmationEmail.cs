using MediatR;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Identity.Clients;
using Notescrib.Identity.Data;
using Notescrib.Identity.Utils;

namespace Notescrib.Identity.Features.Users.Notifications;

public static class SendConfirmationEmail
{
    public record Notification(AppUser User) : INotification;
    
    internal class EmailNotificationHandler : INotificationHandler<Notification>
    {
        private readonly IEmailsApiClient _client;
        private readonly AppUserManager _userManager;

        public EmailNotificationHandler(IEmailsApiClient client, AppUserManager userManager)
        {
            _client = client;
            _userManager = userManager;
        }

        public async Task Handle(Notification notification, CancellationToken cancellationToken)
        {
            var user = notification.User;
        
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var result = await _client.SendConfirmationEmailAsync(user.Email!, user.Id, token);
            if (!result)
            {
                throw new ServerErrorException(ErrorCodes.Emails.EmailsError);
            }
        }
    }
}
