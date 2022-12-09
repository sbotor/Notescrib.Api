﻿using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Folders.Models;
using Notescrib.Notes.Features.Workspaces;

namespace Notescrib.Notes.Features.Folders.Mappers;

public class FolderInfoBaseMapper : IMapper<Folder, FolderInfoBase>
{
    public FolderInfoBase Map(Folder item)
        => new() { Id = item.Id, Name = item.Name, Created = item.Created, Updated = item.Updated };
}
