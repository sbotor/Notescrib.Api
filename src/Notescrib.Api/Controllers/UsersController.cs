using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Api.Application.Users.Models;
using Notescrib.Api.Attributes;
using Notescrib.Api.Contracts.Users;

namespace Notescrib.Api.Controllers;

[ControllerRoute]
[Authorize]
public class UsersController : ApiControllerBase
{
    public UsersController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [AllowAnonymous]
    [CreatedApiResponse]
    public Task<IActionResult> AddUser(CreateUserRequest request)
        => Ok(request.ToCommand());
}
