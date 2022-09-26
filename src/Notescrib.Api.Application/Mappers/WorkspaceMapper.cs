using Notescrib.Api.Application.Contracts.Workspace;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Mappers;

internal class WorkspaceMapper : IWorkspaceMapper
{
    public Workspace MapToEntity(WorkspaceRequest request, string ownerId, Workspace? original = null)
        => new()
        {
            Id = original?.Id,
            Name = request.Name,
            Folders = original?.Folders ?? new List<FolderPath>(),
            OwnerId = ownerId,
            SharingDetails = request.SharingDetails
        };

    public WorkspaceResponse MapToResponse(Workspace workspace, IEnumerable<NoteDetails>? notes = null)
        => new()
        {
            Id = workspace.Id ?? string.Empty,
            Name = workspace.Name,
            SharingDetails = workspace.SharingDetails,
            Folders = workspace.Folders.Select(f => new FolderDetails
            {
                Name = f.Name,
                IsRoot = f.IsRoot,
                Path = f.AbsolutePath,
                Notes = notes?.ToList() ?? new List<NoteDetails>(),
            })
            .ToList()
        };
}
