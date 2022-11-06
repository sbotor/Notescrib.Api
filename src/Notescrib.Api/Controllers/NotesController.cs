﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notescrib.Api.Attributes;
using Notescrib.Api.Contracts.Notes;

namespace Notescrib.Api.Controllers;

[ControllerRoute]
public class NotesController : ApiControllerBase
{
    public NotesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public Task<IActionResult> AddNote(AddNoteRequest request)
        => Ok(request.ToCommand());
}
