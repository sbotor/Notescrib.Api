using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Core.Api.Attributes;
using Notescrib.Identity.Api.Features.Auth.Models;
using Notescrib.Identity.Features.Auth.Models;

namespace Notescrib.Identity.Api.Features.Auth;

[ApiController]
[ApiRoute]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Authenticate(AuthRequest request)
        => Ok(await _mediator.Send(request.ToQuery()));
}
