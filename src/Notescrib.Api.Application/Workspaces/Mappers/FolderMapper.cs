using Notescrib.Api.Application.Notes.Models;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Mappers;

internal class FolderMapper
{
    public FolderResponse MapToResponse(FolderPath folder, IEnumerable<NoteOverviewResponse> notes)
        => new()
        {
            Name = folder.Name,
            AbsolutePath = folder.AbsolutePath,
            WorkspaceId = folder.WorkspaceId,
            IsRoot = folder.IsRoot,
            Notes = notes.ToList()
        };
}
