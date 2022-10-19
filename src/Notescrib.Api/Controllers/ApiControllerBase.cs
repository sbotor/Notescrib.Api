using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Api.Core.Exceptions;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IMediator Mediator { get; }

    public ApiControllerBase(IMediator mediator)
    {
        Mediator = mediator;
    }

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
}
