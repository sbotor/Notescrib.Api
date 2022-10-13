using Microsoft.AspNetCore.Identity;
using Notescrib.Api.Application.Auth.Services;
using Notescrib.Api.Application.Contracts.User;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Infrastructure.Identity;

internal class IdentityAuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;

    public IdentityAuthService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<UserDetails>> AuthenticateAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Result<UserDetails>.NotFound();
        }

        if (!user.EmailConfirmed)
        {
            return Result<UserDetails>.Failure("User does not have a confirmed email.");
        }

        var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
        {
            return Result<UserDetails>.Forbidden();
        }

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            await _userManager.ChangePasswordAsync(user, user.PasswordHash, password);
        }

        return Result<UserDetails>.Success(new UserDetails
        {
            Id = user.Id,
            Email = user.Email
        });
    }
}
