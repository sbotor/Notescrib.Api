using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Api.Core.Exceptions;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ISender Mediator { get; }

    public ApiControllerBase(ISender mediator)
    {
        Mediator = mediator;
    }

    protected IActionResult GetResult<TResponse>(Result<TResponse> response)
        => response.IsSuccessful
            ? StatusCode((int)(response.StatusCode ?? System.Net.HttpStatusCode.OK), response.Response)
            : StatusCode((int)(response.StatusCode ?? System.Net.HttpStatusCode.BadRequest), response.Error);

    protected async Task<IActionResult> GetResponseAsync<TResponse>(IRequest<Result<TResponse>> request)
    {
        Result<TResponse> result;
        try
        {
            result = await Mediator.Send(request);
        }
        catch (AppException e)
        {
            result = e.ToResult<TResponse>();
        }

        return result.IsSuccessful
            ? StatusCode((int)(result.StatusCode ?? System.Net.HttpStatusCode.OK), result.Response)
            : StatusCode((int)(result.StatusCode ?? System.Net.HttpStatusCode.BadRequest), result.Error);
    }

    protected async Task<IActionResult> GetResponseAsync(IRequest<Result> request)
    {
        Result result;
        try
        {
            result = await Mediator.Send(request);
        }
        catch (AppException e)
        {
            result = e.ToResult();
        }

        return result.IsSuccessful
            ? StatusCode((int)(result.StatusCode ?? System.Net.HttpStatusCode.OK))
            : StatusCode((int)(result.StatusCode ?? System.Net.HttpStatusCode.BadRequest), result.Error);
    }
}
