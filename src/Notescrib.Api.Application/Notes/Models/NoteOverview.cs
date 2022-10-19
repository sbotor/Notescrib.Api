﻿using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Notes.Models;

public class NoteOverview
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public IReadOnlyCollection<string> Labels { get; set; } = new List<string>();
    public SharingDetails SharingDetails { get; set; } = new();
}
