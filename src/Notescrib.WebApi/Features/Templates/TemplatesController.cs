using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Features.Templates.Commands;
using Notescrib.Features.Templates.Queries;
using Notescrib.WebApi.Features.Templates.Models;

namespace Notescrib.WebApi.Features.Templates;

[ApiController]
[Route("api/[controller]")]
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
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTemplate(Guid id, CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetNoteTemplateDetails.Query(id), cancellationToken));
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTemplate(Guid id, UpdateNoteTemplateRequest request,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(id), cancellationToken));
    
    [HttpPut("{id:guid}/content")]
    public async Task<IActionResult> UpdateTemplateContent(Guid id, UpdateNoteTemplateContentRequest request,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(request.ToCommand(id), cancellationToken));
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTemplate(Guid id,
        CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new DeleteNoteTemplate.Command(id), cancellationToken));
}
