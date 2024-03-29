﻿using Notescrib.Features.Templates.Commands;

namespace Notescrib.WebApi.Features.Templates.Models;

public class UpdateNoteTemplateContentRequest
{
    public string Content { get; set; } = null!;

    public UpdateNoteTemplateContent.Command ToCommand(Guid id)
        => new(id, Content);
}
