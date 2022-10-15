using Microsoft.AspNetCore.Identity;
using Notescrib.Api.Application.Users;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Exceptions;
using Notescrib.Api.Core.Models;
using Notescrib.Api.Infrastructure.Identity.Models;

namespace Notescrib.Api.Infrastructure.Identity;

internal class UserRepository : IUserRepository
{
    private readonly UserManager<UserData> _userManager;

    public UserRepository(UserManager<UserData> userManager)
    {
        _userManager = userManager;
    }

    public async Task<User> AddUserAsync(User user, string password)
    {
        if (!await ExistsByEmailAsync(user.Email))
        {
            throw new BadRequestException("User with this email already exists.");
        }

        var identityUser = new UserData
        {
            Email = user.Email,
            UserName = user.Email,
            IsActive = user.IsActive,
            EmailConfirmed = true // TODO: Email confirmation
        };

        var result = await _userManager.CreateAsync(identityUser);
        if (!result.Succeeded)
        {
            throw new AppException(GetIdentityErrors(result));
        }

        result = await _userManager.AddPasswordAsync(identityUser, password);

        return result.Succeeded
            ? (await GetUserByEmailAsync(user.Email))!
            : throw new AppException(GetIdentityErrors(result));
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var foundUser = await _userManager.FindByEmailAsync(email);
        return foundUser == null;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        return user == null
            ? null
            : new User
            {
                Email = user.Email,
                Id = user.Id
            };
    }

    private static IEnumerable<ErrorItem> GetIdentityErrors(IdentityResult result)
        => result.Errors.Select(x => new ErrorItem
        {
            Key = x.Code,
            Messages = new List<string> { x.Description }
        });
}
