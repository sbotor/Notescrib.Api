using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Core.Api.Attributes;
using Notescrib.Notes.Api.Features.Templates.Models;

namespace Notescrib.Notes.Api.Features.Templates;

[ApiController]
[ApiRoute]
[Authorize]
public class TemplatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TemplatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTemplate(CreateTemplateRequest request,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(), cancellationToken));

    [HttpGet]
    public async Task<IActionResult> SearchTemplates([FromQuery] SearchTemplatesRequest request,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToQuery(), cancellationToken));
}
