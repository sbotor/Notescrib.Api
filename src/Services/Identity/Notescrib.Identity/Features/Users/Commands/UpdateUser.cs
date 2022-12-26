using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Identity.Data;
using Notescrib.Identity.Utils;

namespace Notescrib.Identity.Features.Users.Commands;

public static class UpdateUser
{
    public record Command(string Email) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly AppUserManager _userManager;
        private readonly IUserContextProvider _userContextProvider;

        public Handler(AppUserManager userManager, IUserContextProvider userContextProvider)
        {
            _userManager = userManager;
            _userContextProvider = userContextProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(_userContextProvider.UserId);
            if (user == null)
            {
                throw new NotFoundException(ErrorCodes.User.UserNotFound);
            }

            if (await _userManager.ExistsByEmailAsync(request.Email))
            {
                throw new DuplicationException(ErrorCodes.User.EmailTaken);
            }

            user.Email = request.Email;
            user.UserName = request.Email;

            await _userManager.UpdateAsync(user);
            
            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
