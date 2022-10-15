using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Api.Application.Auth.Contracts;
using Notescrib.Api.Attributes;
using Notescrib.Api.Contracts.Auth;

namespace Notescrib.Api.Controllers;

[ControllerRoute]
public class AuthController : ApiControllerBase
{
    public AuthController(ISender mediator) : base(mediator)
    {
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Authenticate(LoginRequest request)
        => await GetResponseAsync(request.ToQuery());
}
