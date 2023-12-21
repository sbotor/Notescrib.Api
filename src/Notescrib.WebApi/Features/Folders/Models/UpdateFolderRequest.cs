﻿using Notescrib.Features.Folders.Commands;

namespace Notescrib.WebApi.Features.Folders.Models;

public class UpdateFolderRequest
{
    public string Name { get; set; } = null!;

    public UpdateFolder.Command ToCommand(string id)
        => new(id, Name);
}
