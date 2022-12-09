using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Identity.Clients;
using Notescrib.Identity.Data;
using Notescrib.Identity.Features.Auth.Models;
using Notescrib.Identity.Features.Auth.Providers;
using Notescrib.Identity.Features.Users.Mappers;

namespace Notescrib.Identity.Features.Users.Commands;

public static class CreateUser
{
    public record Command(string Email, string Password, string PasswordConfirmation) : ICommand<TokenResponse>;

    internal class Handler : ICommandHandler<Command, TokenResponse>
    {
        private readonly AppUserManager _userManager;
        private readonly IUserMapper _mapper;
        private readonly IJwtProvider _jwtProvider;
        private readonly INotesApiClient _client;

        public Handler(AppUserManager userManager, IUserMapper mapper, IJwtProvider jwtProvider, INotesApiClient client)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtProvider = jwtProvider;
            _client = client;
        }

        public async Task<TokenResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.Password != request.PasswordConfirmation)
            {
                throw new AppException("Passwords do not match.");
            }

            if (await _userManager.ExistsByEmailAsync(request.Email))
            {
                throw new DuplicationException("User with this email already exists.");
            }

            var user = _mapper.MapToEntity(request);

            var result = await _userManager.CreateAsync(user);
            CheckResultOrThrow(result);

            result = await _userManager.AddPasswordAsync(user, request.Password);

            if (result.Succeeded)
            {
                var jwt = _jwtProvider.GenerateToken(user.Id);
                await CreateWorkspaceOrThrow(user, jwt);
                
                return new TokenResponse
                {
                    Token = jwt, User = _mapper.MapToDetails(user)
                };
            }

            await _userManager.DeleteAsync(user);
            throw new AppException(SerializeErrors(result));
        }

        private async Task CreateWorkspaceOrThrow(AppUser user, string jwt)
        {
            try
            {
                var result = await _client.CreateWorkspace(jwt);
                if (!result)
                {
                    throw new Exception("Problem creating the workspace.");
                }
            }
            catch (Exception)
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
