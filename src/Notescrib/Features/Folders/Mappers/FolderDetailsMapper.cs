using Notescrib.Contracts;
using Notescrib.Features.Folders.Models;
using Notescrib.Features.Notes;
using Notescrib.Features.Notes.Models;

namespace Notescrib.Features.Folders.Mappers;

public class FolderDetailsMapper : IMapper<Folder, FolderDetails>
{
    private readonly IMapper<FolderData, FolderOverview> _baseMapper;
    private readonly IMapper<NoteData, NoteOverview> _noteMapper;

    public FolderDetailsMapper(IMapper<FolderData, FolderOverview> baseMapper, IMapper<NoteData, NoteOverview> noteMapper)
    {
        _baseMapper = baseMapper;
        _noteMapper = noteMapper;
    }
    
    public FolderDetails Map(Folder item)
        => new()
        {
            Id = item.Id,
            Name = item.Name,
            Children = item.ImmediateChildren.Select(_baseMapper.Map).ToArray(),
            Created = item.Created,
            Updated = item.Updated
        };
}
