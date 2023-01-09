using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Identity.Data;
using Notescrib.Identity.Extensions;
using Notescrib.Identity.Utils;

namespace Notescrib.Identity.Features.Users.Commands;

public static class ResetUserPassword
{
    public record Command(string UserId, string Token, string Password, string PasswordConfirmation) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly AppUserManager _userManager;

        public Handler(AppUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                throw new NotFoundException(ErrorCodes.User.UserNotFound);
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
            if (!result.Succeeded)
            {
                throw new AppException(result.ToErrorModels());
            }
            
            return Unit.Value;
        }
    }
    
    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Password)
                .Must(BeEqual)
                .WithErrorCode(ErrorCodes.User.PasswordsDoNotMatch);

            RuleFor(x => x.UserId)
                .NotEmpty();
            
            RuleFor(x => x.Token)
                .NotEmpty();
        }

        private static bool BeEqual(Command command, string _)
            => command.Password == command.PasswordConfirmation;
    }
}
