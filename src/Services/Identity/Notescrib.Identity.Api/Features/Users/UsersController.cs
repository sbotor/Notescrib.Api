using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Core.Api.Attributes;
using Notescrib.Identity.Api.Features.Users.Models;
using Notescrib.Identity.Features.Users.Commands;
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
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
        => Ok(await _mediator.Send(request.ToCommand()));

    [HttpGet]
    public async Task<IActionResult> GetCurrentUserDetails(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetUserDetails.Query(), cancellationToken));
    
    [HttpDelete]
    public async Task<IActionResult> DeleteUser()
        => Ok(await _mediator.Send(new DeleteUser.Command()));
    
    [HttpPost("{id}/confirm")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string id, ConfirmEmailRequest request)
        => Ok(await _mediator.Send(request.ToCommand(id)));
}
