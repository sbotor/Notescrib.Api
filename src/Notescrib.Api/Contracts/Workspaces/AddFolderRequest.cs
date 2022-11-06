﻿using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Contracts.Workspaces;

public class AddFolderRequest : ModifyFolderRequestBase
{
    public CreateFolder.Command ToCommand(string workspaceId)
        => new()
        {
            WorkspaceId = workspaceId,
            Name = Name,
            ParentId = ParentId,
            SharingInfo = SharingInfo
        };
}
