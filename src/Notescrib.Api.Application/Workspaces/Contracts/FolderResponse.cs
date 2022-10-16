﻿using Notescrib.Api.Application.Notes.Models;

namespace Notescrib.Api.Application.Workspaces.Contracts;

public class FolderResponse
{
    public string Name { get; set; } = string.Empty;
    public string AbsolutePath { get; set; } = string.Empty;
    public string WorkspaceId { get; set; } = string.Empty;
    public bool IsRoot { get; set; }
    public IReadOnlyCollection<NoteOverviewResponse> Notes { get; set; } = new List<NoteOverviewResponse>();
}
