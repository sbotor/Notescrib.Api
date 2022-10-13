using Microsoft.AspNetCore.Identity;
using Notescrib.Api.Application.Contracts.User;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Exceptions;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Infrastructure.Identity;

internal class UserRepository
{
    private readonly UserManager<IdentityUser> _userManager;

    public UserRepository(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<User> AddUserAsync(User user, string password, string passwordConfirmation)
    {
        if (password != passwordConfirmation)
        {
            throw new RequestValidationException("Passwords do not match.");
        }

        if (!await ExistsByEmailAsync(user.Email))
        {
            throw new RequestValidationException("User with this email already exists.");
        }

        var identityUser = new IdentityUser
        {
            Email = user.Email,
            UserName = user.Email,
            EmailConfirmed = true // TODO: Email confirmation
        };

        var result = await _userManager.CreateAsync(identityUser);
        if (!result.Succeeded)
        {
            throw new RequestValidationException(GetIdentityErrors(result));
        }

        result = await _userManager.AddPasswordAsync(identityUser, password);

        return result.Succeeded
            ? await GetUserByEmailAsync(request.Email)
            : throw new RequestValidationException(GetIdentityErrors(result));
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var foundUser = await _userManager.FindByEmailAsync(email);
        return foundUser == null;
    }

    private static IEnumerable<ValidationError> GetIdentityErrors(IdentityResult result)
        => result.Errors.Select(x => new ValidationError
        {
            Key = x.Code,
            Messages = new List<string> { x.Description }
        });
}
