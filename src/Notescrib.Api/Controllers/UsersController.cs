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

    //[HttpGet("{email}")]
    //[ProducesResponseType(typeof(UserDetails), (int)HttpStatusCode.OK)]
    //public async Task<IActionResult> GetUserByEmail(string email)
    //    => GetResult(await _userService.GetUserByEmailAsync(email));

    [HttpPost]
    [AllowAnonymous]
    [ApiResponse(typeof(UserDetails), HttpStatusCode.Created)]
    public async Task<IActionResult> AddUser(CreateUserRequest request)
        => await GetResponseAsync(request.ToCommand());
}
