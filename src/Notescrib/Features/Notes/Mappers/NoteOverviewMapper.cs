using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Notes.Mappers;

public class NoteOverviewMapper : IMapper<NoteData, NoteOverview>, IMapper<Note, NoteOverview>,
    IMapper<NoteBase, NoteOverview>
{
    private readonly IPermissionGuard _permissionGuard;

    public NoteOverviewMapper(IPermissionGuard permissionGuard)
    {
        _permissionGuard = permissionGuard;
    }
    
    public NoteOverview Map(NoteData item)
        => Map((NoteBase)item);

    public NoteOverview Map(Note item)
        => Map((NoteBase)item);

    public NoteOverview Map(NoteBase item)
        => new()
        {
            Id = item.Id,
            Name = item.Name,
            FolderId = item.FolderId,
            OwnerId = item.OwnerId,
            SharingInfo = item.SharingInfo,
            Updated = item.Updated,
            Created = item.Created,
            Tags = item.Tags.ToArray(),
            IsReadonly = !_permissionGuard.CanEdit(item.OwnerId)
        };
}
