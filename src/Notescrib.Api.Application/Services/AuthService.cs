using System.Net;
using Microsoft.Extensions.Logging;
using Notescrib.Api.Application.Contracts.User;
using Notescrib.Api.Core;

namespace Notescrib.Api.Application.Services;

internal class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IJwtProvider _jwtProvider;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserService userService, IJwtProvider jwtProvider, ILogger<AuthService> logger)
    {
        _userService = userService;
        _jwtProvider = jwtProvider;
        _logger = logger;
    }

    public async Task<ApiResponse<TokenResponse>> AuthenticateAsync(LoginRequest request)
    {
        var verifyResult = await _userService.VerifyCredentialsAsync(request);

        if (!verifyResult.IsSuccessful || verifyResult.Response == null)
        {
            _logger.LogWarning("Failed login with email {email}.", request.Email);
            return ApiResponse<TokenResponse>.Failure(verifyResult.Error, verifyResult.StatusCode ?? HttpStatusCode.BadRequest);
        }

        var token = _jwtProvider.GenerateToken(verifyResult.Response.Id, verifyResult.Response.Email);

        return ApiResponse<TokenResponse>.Success(new TokenResponse
        {
            Token = token,
            User = verifyResult.Response
        });
    }
}
