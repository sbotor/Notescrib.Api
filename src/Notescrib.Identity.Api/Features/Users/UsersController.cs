using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Identity.Api.Features.Users.Models;
using Notescrib.Identity.Features.Auth.Models;
using Notescrib.Identity.Features.Users.Commands;
using Notescrib.Identity.Features.Users.Models;
using Notescrib.Identity.Features.Users.Queries;

namespace Notescrib.Identity.Api.Features.Users;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public Task<TokenResponse> CreateUser(CreateUserRequest request)
        => _mediator.Send(request.ToCommand());

    [HttpGet]
    public Task<UserDetails> GetCurrentUserDetails(CancellationToken cancellationToken)
        => _mediator.Send(new GetUserDetails.Query(), cancellationToken);
    
    [HttpDelete]
    public Task DeleteUser()
        => _mediator.Send(new DeleteUser.Command());
    
    [HttpPost("{id}/activate")]
    [AllowAnonymous]
    public Task ActivateAccount(string id, ActivateAccountRequest request)
        => _mediator.Send(request.ToCommand(id));
    
    [HttpPost("password")]
    [AllowAnonymous]
    public Task InitiatePasswordReset(InitiatePasswordResetRequest request)
        => _mediator.Send(request.ToCommand());
    
    [HttpPut("{id}/password")]
    [AllowAnonymous]
    public Task ResetPassword(string id, ResetUserPasswordRequest request)
        => _mediator.Send(request.ToCommand(id));
}
