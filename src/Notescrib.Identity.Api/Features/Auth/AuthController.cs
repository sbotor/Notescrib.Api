using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Identity.Api.Features.Auth.Models;
using Notescrib.Identity.Features.Auth.Models;

namespace Notescrib.Identity.Api.Features.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    public Task<TokenResponse> Authenticate(AuthRequest request)
        => _mediator.Send(request.ToQuery());
}
