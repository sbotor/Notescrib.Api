using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Api.Application.Contracts.User;
using Notescrib.Api.Application.Services;

namespace Notescrib.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class UsersController : ApiControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{email}")]
    [ProducesResponseType(typeof(UserDetails), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUserByEmail(string email)
        => GetResult(await _userService.GetUserByEmailAsync(email));

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserDetails), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddUser(CreateUserRequest request)
        => GetResult(await _userService.AddUserAsync(request));
}
