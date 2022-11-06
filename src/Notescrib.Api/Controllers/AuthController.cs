using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Api.Application.Auth.Models;
using Notescrib.Api.Attributes;
using Notescrib.Api.Contracts.Auth;

namespace Notescrib.Api.Controllers;

[ControllerRoute]
public class AuthController : ApiControllerBase
{
    public AuthController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), (int)HttpStatusCode.OK)]
    public Task<IActionResult> Authenticate(LoginRequest request)
        => Ok(request.ToQuery());
}
