﻿using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Contracts.Workspaces;

public class AddWorkspaceRequest
{
    public string Name { get; set; } = null!;
    public SharingInfo? SharingInfo { get; set; }

    public AddWorkspace.Command ToCommand()
        => new(Name, SharingInfo ?? new());
}
