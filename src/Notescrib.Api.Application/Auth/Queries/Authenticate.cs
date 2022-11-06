using Microsoft.Extensions.Logging;
using Notescrib.Api.Application.Cqrs;
using System.Net;
using Notescrib.Api.Application.Auth.Services;
using Notescrib.Api.Application.Auth.Contracts;
using Notescrib.Api.Core.Models;
using Notescrib.Api.Application.Users.Mappers;
using Notescrib.Api.Application.Users.Models;
using Notescrib.Api.Core.Exceptions;

namespace Notescrib.Api.Application.Auth.Queries;

public static class Authenticate
{
    public record Query(string Email, string Password) : IQuery<TokenResponse>;

    internal class Handler : IQueryHandler<Query, TokenResponse>
    {
        private readonly IAuthService _authService;
        private readonly IJwtProvider _jwtProvider;
        private readonly IUserMapper _mapper;

        public Handler(IAuthService userService, IJwtProvider jwtProvider, IUserMapper mapper)
        {
            _authService = userService;
            _jwtProvider = jwtProvider;
            _mapper = mapper;
        }

        public async Task<TokenResponse> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _authService.AuthenticateAsync(request.Email, request.Password);
            if (user == null)
            {
                throw new AppException("Could not authenticate.");
            }

            var token = _jwtProvider.GenerateToken(user.Id, user.Email);

            return new TokenResponse
            {
                Token = token,
                User = _mapper.Map<UserDetails>(user)
            };
        }
    }
}
