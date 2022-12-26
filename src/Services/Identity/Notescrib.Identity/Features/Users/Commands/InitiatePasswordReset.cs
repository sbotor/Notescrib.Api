using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Identity.Clients;
using Notescrib.Identity.Data;
using Notescrib.Identity.Utils;

namespace Notescrib.Identity.Features.Users.Commands;

public static class InitiatePasswordReset
{
    public record Command : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly AppUserManager _userManager;
        private readonly IUserContextProvider _userContextProvider;
        private readonly IEmailsApiClient _client;

        public Handler(AppUserManager userManager, IUserContextProvider userContextProvider, IEmailsApiClient client)
        {
            _userManager = userManager;
            _userContextProvider = userContextProvider;
            _client = client;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(_userContextProvider.UserId);
            if (user == null)
            {
                throw new NotFoundException(ErrorCodes.User.UserNotFound);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _client.SendResetPasswordEmailAsync(user.Email!, user.Id, token);

            return Unit.Value;
        }
    }
}
