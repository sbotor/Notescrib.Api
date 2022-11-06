using MediatR;
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

    protected async Task<IActionResult> Ok<TResponse>(IRequest<TResponse> request)
        => Ok(await _mediator.Send(request));

    protected async Task<IActionResult> NoContent(IRequest request)
    {
        await _mediator.Send(request);
        return new NoContentResult();
    }

    protected async Task<IActionResult> CreatedAtAction(IRequest<string> request, string actionName)
    {
        var result = await _mediator.Send(request);
        return CreatedAtAction(actionName, new[] { result }, null);
    }
}
