using MediatR;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Identity.Clients;
using Notescrib.Identity.Data;
using Notescrib.Identity.Features.Emails.Services;
using Notescrib.Identity.Utils;

namespace Notescrib.Identity.Features.Users.Notifications;

public static class SendConfirmationEmail
{
    public record Notification(AppUser User) : INotification;
    
    internal class EmailNotificationHandler : INotificationHandler<Notification>
    {
        private readonly IEmailSender _emailSender;
        private readonly AppUserManager _userManager;

        public EmailNotificationHandler(IEmailSender emailSender, AppUserManager userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public async Task Handle(Notification notification, CancellationToken cancellationToken)
        {
            var user = notification.User;
        
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _emailSender.SendActivationEmailAsync(user.Email!, user.Id, token);
        }
    }
}
