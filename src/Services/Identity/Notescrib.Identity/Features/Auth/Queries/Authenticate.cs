using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Identity.Data;
using Notescrib.Identity.Features.Auth.Models;
using Notescrib.Identity.Features.Auth.Providers;
using Notescrib.Identity.Features.Users.Mappers;

namespace Notescrib.Identity.Features.Auth.Queries;

public static class Authenticate
{
    public record Query(string Email, string Password) : IQuery<TokenResponse>;

    internal class Handler : IQueryHandler<Query, TokenResponse>
    {
        private readonly AppUserManager _userManager;
        private readonly IJwtProvider _jwtProvider;
        private readonly IUserMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(AppUserManager userManager, IJwtProvider jwtProvider, IUserMapper mapper, ILogger<Handler> logger)
        {
            _userManager = userManager;
            _jwtProvider = jwtProvider;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TokenResponse> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new NotFoundException();
            }

            if (!user.EmailConfirmed)
            {
                throw new AppException();
            }
            
            var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);
        
            switch (result)
            {
                case PasswordVerificationResult.Failed:
                    _logger.LogWarning("Failed login with email {email}.", request.Email);
                    throw new ForbiddenException();
            
                case PasswordVerificationResult.SuccessRehashNeeded:
                    await _userManager.ChangePasswordAsync(user, user.PasswordHash!, request.Password);
                    break;
            }
            
            return new TokenResponse
            {
                Token = _jwtProvider.GenerateToken(user.Id),
                User = _mapper.MapToDetails(user)
            };
        }
    }
}
