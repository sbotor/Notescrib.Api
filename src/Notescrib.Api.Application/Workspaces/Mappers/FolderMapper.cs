using Notescrib.Api.Application.Notes.Models;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Mappers;

internal class FolderMapper : IFolderMapper
{
    public FolderDetails MapToResponse(Folder folder, IEnumerable<NoteOverview> notes)
        => new()
        {
            Name = folder.Name,
            ParentPath = folder.AbsolutePath,
            WorkspaceId = folder.WorkspaceId,
            IsRoot = folder.IsRoot,
            Notes = notes.ToList(),
            SharingDetails = folder.SharingDetails
        };
}
