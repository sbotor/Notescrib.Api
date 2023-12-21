using Notescrib.Contracts;
using Notescrib.Features.Notes.Models;
using Notescrib.Services;

namespace Notescrib.Features.Notes.Mappers;

public class NoteDetailsMapper : IMapper<Note, NoteDetails>
{
    private readonly IMapper<NoteBase, NoteOverview> _baseMapper;
    private readonly IPermissionGuard _permissionGuard;

    public NoteDetailsMapper(IMapper<NoteBase, NoteOverview> baseMapper, IPermissionGuard permissionGuard)
    {
        _baseMapper = baseMapper;
        _permissionGuard = permissionGuard;
    }
    
    public NoteDetails Map(Note item)
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
            Content = item.Content,
            Related = item.Related.Select(_baseMapper.Map).ToArray(),
            IsReadonly = !_permissionGuard.CanEdit(item.OwnerId)
        };
}
