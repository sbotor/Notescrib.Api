using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Api.Application.Contracts.User;
using Notescrib.Api.Application.Services.Auth;

namespace Notescrib.Api.Controllers;

[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Authenticate(LoginRequest request)
        => GetResult(await _authService.AuthenticateAsync(request));
}
