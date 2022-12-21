using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Identity.Data;
using Notescrib.Identity.Features.Auth.Models;
using Notescrib.Identity.Features.Auth.Providers;
using Notescrib.Identity.Features.Users.Mappers;
using Notescrib.Identity.Features.Users.Notifications;
using Notescrib.Identity.Utils;

namespace Notescrib.Identity.Features.Users.Commands;

public static class CreateUser
{
    public record Command(string Email, string Password, string PasswordConfirmation) : ICommand<TokenResponse>;

    internal class Handler : ICommandHandler<Command, TokenResponse>
    {
        private readonly AppUserManager _userManager;
        private readonly IUserMapper _mapper;
        private readonly IJwtProvider _jwtProvider;
        private readonly IMediator _mediator;

        public Handler(AppUserManager userManager, IUserMapper mapper, IJwtProvider jwtProvider, IMediator mediator)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtProvider = jwtProvider;
            _mediator = mediator;
        }

        public async Task<TokenResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.Password != request.PasswordConfirmation)
            {
                throw new AppException(ErrorCodes.User.PasswordsDoNotMatch);
            }

            if (await _userManager.ExistsByEmailAsync(request.Email))
            {
                throw new DuplicationException(ErrorCodes.User.EmailTaken);
            }

            var user = _mapper.MapToEntity(request);

            var result = await _userManager.CreateAsync(user);
            CheckResultOrThrow(result);

            result = await _userManager.AddPasswordAsync(user, request.Password);

            if (result.Succeeded)
            {
                var jwt = _jwtProvider.GenerateToken(user.Id);

                await CreateWorkspaceAndSendEmail(user, jwt);
                
                return new TokenResponse
                {
                    Token = jwt, User = _mapper.MapToDetails(user)
                };
            }

            await _userManager.DeleteAsync(user);
            throw new AppException(SerializeErrors(result));
        }

        private async Task CreateWorkspaceAndSendEmail(AppUser user, string jwt)
        {
            try
            {
                await _mediator.Publish(new CreateWorkspace.Notification(jwt), CancellationToken.None);
                
                try
                {
                    await _mediator.Publish(new SendConfirmationEmail.Notification(user), CancellationToken.None);
                }
                catch
                {
                    await _mediator.Publish(new DeleteWorkspace.Notification(jwt));
                    throw;
                }
            }
            catch
            {
                await _userManager.DeleteAsync(user);
                throw;
            }
        }

        private static void CheckResultOrThrow(IdentityResult result)
        {
            if (result.Succeeded)
            {
                return;
            }
            
            throw new AppException(SerializeErrors(result));
        }

        private static string SerializeErrors(IdentityResult result)
        {
            var errors = result.Errors.ToDictionary(x => x.Code, x => x.Description);
            return JsonSerializer.Serialize(errors);
        }
    }
}
