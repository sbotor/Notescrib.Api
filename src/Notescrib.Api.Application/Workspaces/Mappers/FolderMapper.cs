using Notescrib.Api.Application.Notes.Models;
using Notescrib.Api.Application.Workspaces.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Mappers;

internal class FolderMapper
{
    public FolderResponse MapToResponse(FolderPath folder, IEnumerable<NoteOverviewResponse> notes)
        => new()
        {
            Name = folder.Name,
            Path = folder.AbsolutePath,
            IsRoot = folder.IsRoot,
            Notes = notes.ToList()
        };
}
