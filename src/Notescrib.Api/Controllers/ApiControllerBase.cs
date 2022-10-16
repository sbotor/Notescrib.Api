using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Api.Application.Extensions;
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

    protected async Task<IActionResult> GetResponseAsync<TResponse>(IRequest<Result<TResponse>> request)
    {
        Result<TResponse> result;
        try
        {
            await ValidateRequestOrThrow(request);
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
            await ValidateRequestOrThrow(request);
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

    private async Task ValidateRequestOrThrow<TRequest>(TRequest request)
    {
        var validators = HttpContext.RequestServices.GetServices<IValidator<TRequest>>();
        if (!validators.Any())
        {
            return;
        }

        var errors = await validators.ValidateAsync(request);

        if (errors.Any())
        {
            throw new RequestValidationException(errors);
        }
    }
}
