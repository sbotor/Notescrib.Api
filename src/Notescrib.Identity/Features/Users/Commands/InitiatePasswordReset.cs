using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Services;
using Notescrib.Identity.Data;
using Notescrib.Identity.Features.Emails.Services;

namespace Notescrib.Identity.Features.Users.Commands;

public static class InitiatePasswordReset
{
    public record Command(string? Email) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly AppUserManager _userManager;
        private readonly IUserContext _userContext;
        private readonly IEmailSender _emailSender;

        public Handler(AppUserManager userManager, IUserContext userContext, IEmailSender emailSender)
        {
            _userManager = userManager;
            _userContext = userContext;
            _emailSender = emailSender;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var userInfo = await _userContext.GetUserInfo(CancellationToken.None);
            
            var user = string.IsNullOrEmpty(request.Email)
                ? await _userManager.FindByIdAsync(userInfo.UserId)
                : await _userManager.FindByEmailAsync(request.Email);
            
            if (user == null)
            {
                return Unit.Value;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailSender.SendPasswordResetEmailAsync(user.Email!, user.Id, token);

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator(IUserContext userContext)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WhenAsync(async (_, _) =>
                {
                    var user = await userContext.GetUserInfo(CancellationToken.None);
                    return user.IsAnonymous;
                });
        }
    }
}
