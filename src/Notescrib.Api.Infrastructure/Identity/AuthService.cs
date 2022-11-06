using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Notescrib.Api.Application.Auth.Services;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Exceptions;
using Notescrib.Api.Infrastructure.Identity.Mappers;
using Notescrib.Api.Infrastructure.Identity.Models;

namespace Notescrib.Api.Infrastructure.Identity;

internal class AuthService : IAuthService
{
    private readonly UserManager<UserData> _userManager;
    private readonly IUserDataMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(UserManager<UserData> userManager, IUserDataMapper mapper, ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new NotFoundException();
        }

        if (!user.EmailConfirmed)
        {
            return null;
        }

        var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        
        switch (result)
        {
            case PasswordVerificationResult.Failed:
                _logger.LogWarning("Failed login with email {email}.", email);
                throw new ForbiddenException();
            
            case PasswordVerificationResult.SuccessRehashNeeded:
                await _userManager.ChangePasswordAsync(user, user.PasswordHash, password);
                break;
        }

        return _mapper.MapToEntity(user);
    }
}
