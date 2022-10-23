﻿using Notescrib.Api.Application.Notes.Models;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Models;

public class FolderDetails : IShareable
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ParentPath { get; set; } = null!;
    public bool IsRoot { get; set; }
    public SharingDetails SharingDetails { get; set; } = null!;
}
