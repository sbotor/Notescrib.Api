using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Folders.Models;
using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Features.Workspaces;

namespace Notescrib.Notes.Features.Folders.Mappers;

public class FolderDetailsMapper : IMapper<Folder, FolderDetails>
{
    private readonly IMapper<FolderData, FolderInfoBase> _baseMapper;
    private readonly IMapper<Note, NoteOverview> _noteMapper;

    public FolderDetailsMapper(IMapper<FolderData, FolderInfoBase> baseMapper, IMapper<Note, NoteOverview> noteMapper)
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
            Updated = item.Updated,
            Notes = item.Notes.Select(_noteMapper.Map).ToArray()
        };
}
