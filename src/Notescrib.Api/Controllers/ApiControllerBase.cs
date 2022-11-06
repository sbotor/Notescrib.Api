﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Api.Core.Exceptions;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    private readonly IMediator _mediator;

    protected ApiControllerBase(IMediator mediator)
    {
        _mediator = mediator;
    }

    protected async Task<IActionResult> GetResponseAsync<TResponse>(IRequest<Result<TResponse>> request)
    {
        var result = await GetResult(request);
        return result.IsSuccessful ? Success(result) : Error(result);
    }

    protected async Task<IActionResult> GetResponseAsync(IRequest<Result> request)
    {
        var result = await GetResult(request);
        return result.IsSuccessful ? Success(result) : Error(result);
    }

    protected async Task<IActionResult> GetCreatedResponseAsync(IRequest<Result<string>> request, string actionName)
    {
        var result = await GetResult(request);
        return result.IsSuccessful
            ? CreatedAtAction(actionName, new { id = result.Response! }, null)
            : Error(result);
    }

    private async Task<Result<TResponse>> GetResult<TResponse>(IRequest<Result<TResponse>> request)
    {
        Result<TResponse> result;
        try
        {
            result = await _mediator.Send(request);
        }
        catch (AppException e)
        {
            result = e.ToResult<TResponse>();
        }

        return result;
    }

    private async Task<Result> GetResult(IRequest<Result> request)
    {
        Result result;
        try
        {
            result = await _mediator.Send(request);
        }
        catch (AppException e)
        {
            result = e.ToResult();
        }

        return result;
    }

    protected IActionResult Success<TResponse>(Result<TResponse> result)
        => StatusCode((int)(result.StatusCode ?? System.Net.HttpStatusCode.OK), result.Response);

    protected IActionResult Error(Result result)
        => StatusCode((int)(result.StatusCode ?? System.Net.HttpStatusCode.BadRequest), result.Error);

    protected IActionResult Success(Result result)
        => StatusCode((int)(result.StatusCode ?? System.Net.HttpStatusCode.NoContent));
}
