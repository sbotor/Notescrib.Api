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

    [HttpPost("confirmation")]
    public async Task<IActionResult> SendConfirmationEmail(SendConfirmationEmailRequest request)
        => Ok(await _mediator.Send(request.ToCommand()));
    
    [HttpPost("password")]
    public async Task<IActionResult> SendPasswordResetEmail(SendConfirmationEmailRequest request)
        => Ok(await _mediator.Send(request.ToCommand()));
}
