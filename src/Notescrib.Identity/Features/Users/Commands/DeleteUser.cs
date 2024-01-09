using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Identity.Data;
using Notescrib.Identity.Features.Auth.Providers;
using Notescrib.Identity.Features.Users.Notifications;
using Notescrib.Identity.Utils;

namespace Notescrib.Identity.Features.Users.Commands;

public static class DeleteUser
{
    public record Command : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly AppUserManager _userManager;
        private readonly IUserContext _userContext;
        private readonly IJwtProvider _jwtProvider;
        private readonly IMediator _mediator;
        private readonly IClock _clock;

        public Handler(AppUserManager userManager, IUserContext userContext, IJwtProvider jwtProvider,
            IMediator mediator, IClock clock)
        {
            _userManager = userManager;
            _userContext = userContext;
            _jwtProvider = jwtProvider;
            _mediator = mediator;
            _clock = clock;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var userInfo = await _userContext.GetUserInfo(CancellationToken.None);
            
            var user = await _userManager.FindByIdAsync(userInfo.UserId);
            if (user == null)
            {
                throw new NotFoundException(ErrorCodes.User.UserNotFound);
            }

            var jwt = _jwtProvider.GenerateToken(userInfo.UserId);

            await _mediator.Publish(new DeleteWorkspace.Notification(jwt), CancellationToken.None);
            
            var deletedDateTime = _clock.Now.ToString("yyyy-MM-ddThh-mm-ss");
            
            user.IsActive = false;
            user.UserName = $"{user.Email}-{deletedDateTime}";

            await _userManager.UpdateAsync(user);

            return Unit.Value;
        }
    }
}
