using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Features.Templates.Commands;
using Notescrib.Features.Templates.Models;
using Notescrib.Features.Templates.Queries;
using Notescrib.Models;
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
    public Task CreateTemplate(CreateTemplateRequest request,
        CancellationToken cancellationToken)
        => _mediator.Send(request.ToCommand(), cancellationToken);

    [HttpGet]
    public Task<PagedList<NoteTemplateOverview>> SearchTemplates([FromQuery] SearchTemplatesRequest request,
        CancellationToken cancellationToken)
        => _mediator.Send(request.ToQuery(), cancellationToken);
    
    [HttpGet("{id:guid}")]
    public Task<NoteTemplateDetails> GetTemplate(Guid id, CancellationToken cancellationToken)
        => _mediator.Send(new GetNoteTemplateDetails.Query(id), cancellationToken);
    
    [HttpPut("{id:guid}")]
    public Task UpdateTemplate(Guid id, UpdateNoteTemplateRequest request,
        CancellationToken cancellationToken)
        => _mediator.Send(request.ToCommand(id), cancellationToken);
    
    [HttpPut("{id:guid}/content")]
    public Task UpdateTemplateContent(Guid id, UpdateNoteTemplateContentRequest request,
        CancellationToken cancellationToken)
        => _mediator.Send(request.ToCommand(id), cancellationToken);
    
    [HttpDelete("{id:guid}")]
    public Task DeleteTemplate(Guid id,
        CancellationToken cancellationToken)
        => _mediator.Send(new DeleteNoteTemplate.Command(id), cancellationToken);
}
