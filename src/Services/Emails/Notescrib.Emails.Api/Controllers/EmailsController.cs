using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Core.Api.Attributes;
using Notescrib.Emails.Api.Models;

namespace Notescrib.Emails.Api.Controllers;

[ApiController]
[ApiRoute]
public class EmailsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmailsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> SendEmailConfirmation(SendEmailConfirmationRequest request)
        => Ok(await _mediator.Send(request.ToCommand()));
}
