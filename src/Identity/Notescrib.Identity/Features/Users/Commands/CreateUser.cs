using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Notescrib.Identity.Data;
using Notescrib.Identity.Exceptions;
using Notescrib.Identity.Features.Auth.Models;
using Notescrib.Identity.Features.Auth.Providers;
using Notescrib.Identity.Features.Users.Mappers;

namespace Notescrib.Identity.Features.Users.Commands;

public static class CreateUser
{
    public record Command(string Email, string Password, string PasswordConfirmation) : IRequest<TokenResponse>;

    internal class Handler : IRequestHandler<Command, TokenResponse>
    {
        private readonly AppUserManager _userManager;
        private readonly IUserMapper _mapper;
        private readonly IJwtProvider _jwtProvider;

        public Handler(AppUserManager userManager, IUserMapper mapper, IJwtProvider jwtProvider)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtProvider = jwtProvider;
        }

        public async Task<TokenResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.Password != request.PasswordConfirmation)
            {
                throw new AppException("Passwords do not match.");
            }

            if (await _userManager.ExistsByEmailAsync(request.Email))
            {
                throw new NotFoundException("User with this email already exists.");
            }

            var user = _mapper.MapToEntity(request);

            var result = await _userManager.CreateAsync(user);
            CheckResultOrThrow(result);

            result = await _userManager.AddPasswordAsync(user, request.Password);
            CheckResultOrThrow(result);

            return new TokenResponse
            {
                Token = _jwtProvider.GenerateToken(user.Id, user.Email),
                User = _mapper.MapToDetails(user)
            };
        }

        private void CheckResultOrThrow(IdentityResult result)
        {
            if (result.Succeeded)
            {
                return;
            }

            var errors = result.Errors.ToDictionary(x => x.Code, x => x.Description);
            throw new AppException(JsonSerializer.Serialize(errors));
        }
    }
}
