using System.Net;
using DnsClient.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Notescrib.Api.Application.Contracts.User;
using Notescrib.Api.Application.Services;
using Notescrib.Api.Core;

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

    public async Task<ApiResponse<UserDetails>> AddUserAsync(CreateUserRequest request)
    {
        if (request.Password != request.PasswordConfirmation)
        {
            return ApiResponse<UserDetails>.Failure("Passwords do not match.");
        }

        try
        {
            if (!await CheckEmailInternal(request.Email))
            {
                return ApiResponse<UserDetails>.Failure("This email is taken.");
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
            return ApiResponse<UserDetails>.Failure(statusCode: HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ApiResponse<bool>> CheckEmailAsync(string email)
        => ApiResponse<bool>.Success(await CheckEmailInternal(email));

    public async Task<ApiResponse<UserDetails>> GetUserByEmailAsync(string email)
    {
        if (!email.Equals(_userContextService.Email, StringComparison.InvariantCultureIgnoreCase))
        {
            return ApiResponse<UserDetails>.Forbidden();
        }

        var user = await _userManager.FindByEmailAsync(email);

        return user == null
            ? ApiResponse<UserDetails>.NotFound()
            : ApiResponse<UserDetails>.Success(new UserDetails
            {
                Id = user.Id,
                Email = user.Email
            });
    }

    public async Task<ApiResponse<UserDetails>> VerifyCredentialsAsync(LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return ApiResponse<UserDetails>.Failure("Invalid login details.");
        }

        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return ApiResponse<UserDetails>.NotFound();
            }

            if (!user.EmailConfirmed)
            {
                return ApiResponse<UserDetails>.Failure("User does not have a confirmed email.");
            }

            var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return ApiResponse<UserDetails>.Forbidden();
            }

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                await _userManager.ChangePasswordAsync(user, user.PasswordHash, request.Password);
            }

            return ApiResponse<UserDetails>.Success(new UserDetails
            {
                Id = user.Id,
                Email = user.Email
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Problem verifying user credentials.");
            return ApiResponse<UserDetails>.Failure(statusCode: HttpStatusCode.InternalServerError);
        }
    }

    private async Task<bool> CheckEmailInternal(string email)
    {
        var foundUser = await _userManager.FindByEmailAsync(email);
        return foundUser == null;
    }

    private static ApiResponse<TResponse> GetIdentityErrors<TResponse>(IdentityResult result)
        => ApiResponse<TResponse>.Failure(string.Join('\n', result.Errors.Select(e => e.Description)));
}
