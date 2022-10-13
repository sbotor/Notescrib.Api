using Microsoft.Extensions.Logging;
using Notescrib.Api.Application.Cqrs;
using System.Net;
using Notescrib.Api.Application.Auth.Services;
using Notescrib.Api.Application.Auth.Contracts;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Auth.Queries;

public static class Authenticate
{
    public record Query(string Email, string Password) : IQuery<Result<TokenResponse>>;

    internal class Handler : IQueryHandler<Query, Result<TokenResponse>>
    {
        private readonly IAuthService _authService;
        private readonly IJwtProvider _jwtProvider;
        private readonly ILogger<Handler> _logger;

        public Handler(IAuthService userService, IJwtProvider jwtProvider, ILogger<Handler> logger)
        {
            _authService = userService;
            _jwtProvider = jwtProvider;
            _logger = logger;
        }

        public async Task<Result<TokenResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var verifyResult = await _authService.AuthenticateAsync(request.Email, request.Password);

            if (!verifyResult.IsSuccessful || verifyResult.Response == null)
            {
                _logger.LogWarning("Failed login with email {email}.", request.Email);
                return Result<TokenResponse>.Failure(verifyResult.Error, verifyResult.StatusCode ?? HttpStatusCode.BadRequest);
            }

            var token = _jwtProvider.GenerateToken(verifyResult.Response.Id, verifyResult.Response.Email);

            return Result<TokenResponse>.Success(new TokenResponse
            {
                Token = token,
                User = verifyResult.Response
            });
        }
    }
}
