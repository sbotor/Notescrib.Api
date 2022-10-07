using Microsoft.AspNetCore.Mvc;
using Notescrib.Api.Core;

namespace Notescrib.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    [NonAction]
    protected IActionResult GetResult<TResponse>(ApiResponse<TResponse> response)
        => response.IsSuccessful
            ? StatusCode((int)(response.StatusCode ?? System.Net.HttpStatusCode.OK), response.Response)
            : StatusCode((int)(response.StatusCode ?? System.Net.HttpStatusCode.BadRequest), response.Error);
}
