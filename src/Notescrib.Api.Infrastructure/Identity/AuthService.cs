using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Notescrib.Api.Application.Auth.Services;
using Notescrib.Api.Application.Users.Models;
using Notescrib.Api.Core.Models;
using Notescrib.Api.Infrastructure.Identity.Models;

namespace Notescrib.Api.Infrastructure.Identity;

internal class AuthService : IAuthService
{
    private readonly UserManager<UserData> _userManager;
    private readonly IMapper _mapper;

    public AuthService(UserManager<UserData> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
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

        return Result<UserDetails>.Success(_mapper.Map<UserDetails>(user));
    }
}
