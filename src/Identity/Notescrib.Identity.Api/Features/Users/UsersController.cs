using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Core.Api.Attributes;
using Notescrib.Identity.Api.Features.Users.Models;
using Notescrib.Identity.Features.Users.Models;
using Notescrib.Identity.Features.Users.Queries;

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

    [HttpGet]
    [ProducesResponseType(typeof(UserDetails), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentUserDetails()
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return BadRequest();
        }
        
        return Ok(await _mediator.Send(new GetUserDetails.Query(userId.Value)));
    }
}
