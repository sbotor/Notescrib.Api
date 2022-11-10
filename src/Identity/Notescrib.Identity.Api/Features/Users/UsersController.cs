using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Core.Api.Attributes;
using Notescrib.Identity.Api.Features.Users.Models;

namespace Notescrib.Identity.Api.Features.Users;

[ApiController]
[ApiRoute]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
        => Ok(await _mediator.Send(request.ToCommand()));
}
