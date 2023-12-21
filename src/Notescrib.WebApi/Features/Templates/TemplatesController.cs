using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Core.Api.Attributes;
using Notescrib.Notes.Api.Features.Templates.Models;
using Notescrib.Notes.Features.Templates.Commands;
using Notescrib.Notes.Features.Templates.Queries;

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
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTemplate(string id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetNoteTemplateDetails.Query(id), cancellationToken));
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTemplate(string id, UpdateNoteTemplateRequest request,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(id), cancellationToken));
    
    [HttpPut("{id}/content")]
    public async Task<IActionResult> UpdateTemplateContent(string id, UpdateNoteTemplateContentRequest request,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(id), cancellationToken));
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTemplate(string id,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new DeleteNoteTemplate.Command(id), cancellationToken));
}
