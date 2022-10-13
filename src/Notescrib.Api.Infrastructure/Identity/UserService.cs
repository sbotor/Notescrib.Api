using System.Net;
using DnsClient.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Notescrib.Api.Application.Common.Services;
using Notescrib.Api.Application.Contracts.User;
using Notescrib.Api.Application.Services;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Infrastructure.Identity;

internal class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserContextService _userContextService;
    private readonly ILogger<UserService> _logger;

    public UserService(UserManager<IdentityUser> userManager, IUserContextService userContextService, ILogger<UserService> logger)
    {
        _userManager = userManager;
        _userContextService = userContextService;
        _logger = logger;
    }

    public async Task<Result<UserDetails>> AddUserAsync(CreateUserRequest request)
    {
        if (request.Password != request.PasswordConfirmation)
        {
            return Result<UserDetails>.Failure("Passwords do not match.");
        }

        try
        {
            if (!await CheckEmailInternal(request.Email))
            {
                return Result<UserDetails>.Failure("This email is taken.");
            }

            var user = new IdentityUser
            {
                Email = request.Email,
                UserName = request.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetIdentityErrors<UserDetails>(result);
            }

            result = await _userManager.AddPasswordAsync(user, request.Password);

            return result.Succeeded
                ? await GetUserByEmailAsync(request.Email)
                : GetIdentityErrors<UserDetails>(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception when adding a new user.");
            return Result<UserDetails>.Failure(statusCode: HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<bool>> CheckEmailAsync(string email)
        => Result<bool>.Success(await CheckEmailInternal(email));

    public async Task<Result<UserDetails>> GetUserByEmailAsync(string email)
    {
        if (!email.Equals(_userContextService.Email, StringComparison.InvariantCultureIgnoreCase))
        {
            return Result<UserDetails>.Forbidden();
        }

        var user = await _userManager.FindByEmailAsync(email);

        return user == null
            ? Result<UserDetails>.NotFound()
            : Result<UserDetails>.Success(new UserDetails
            {
                Id = user.Id,
                Email = user.Email
            });
    }



    private static Result<TResponse> GetIdentityErrors<TResponse>(IdentityResult result)
        => Result<TResponse>.Failure(string.Join('\n', result.Errors.Select(e => e.Description)));
}
