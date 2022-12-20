using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Notes.Models;

namespace Notescrib.Notes.Features.Notes.Mappers;

public class NoteDetailsMapper : IMapper<Note, NoteDetails>
{
    private readonly IMapper<NoteBase, NoteOverview> _baseMapper;

    public NoteDetailsMapper(IMapper<NoteBase, NoteOverview> baseMapper)
    {
        _baseMapper = baseMapper;
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
            Related = item.Related.Select(_baseMapper.Map).ToArray()
        };
}
