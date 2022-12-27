using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Identity.Data;
using Notescrib.Identity.Utils;

namespace Notescrib.Identity.Features.Users.Commands;

public static class ActivateAccount
{
    public record Command(string UserId, string Token) : ICommand;

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

            if (user.EmailConfirmed)
            {
                throw new AppException(ErrorCodes.User.EmailAlreadyConfirmed);
            }

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);
            
            if (result.Succeeded)
            {
                return Unit.Value;
            }

            var errors = result.Errors.Select(x => new ErrorModel(x.Code, x.Description));
            throw new AppException(errors);
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Token)
                .NotEmpty();

            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }
}
