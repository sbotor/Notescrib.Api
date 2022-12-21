﻿using MediatR;
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
        private readonly IUserContextProvider _userContextProvider;
        private readonly IJwtProvider _jwtProvider;
        private readonly IMediator _mediator;

        public Handler(AppUserManager userManager, IUserContextProvider userContextProvider, IJwtProvider jwtProvider, IMediator mediator)
        {
            _userManager = userManager;
            _userContextProvider = userContextProvider;
            _jwtProvider = jwtProvider;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(_userContextProvider.UserId);
            if (user == null)
            {
                throw new NotFoundException(ErrorCodes.User.UserNotFound);
            }
            
            var jwt = _jwtProvider.GenerateToken(_userContextProvider.UserId);

            await _mediator.Publish(new DeleteWorkspace.Notification(jwt), CancellationToken.None);

            user.IsActive = false;
            
            await _userManager.UpdateAsync(user);
            
            return Unit.Value;
        }
    }
}
