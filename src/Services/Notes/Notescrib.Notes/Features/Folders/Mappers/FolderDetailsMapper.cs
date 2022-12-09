using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Folders.Models;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Features.Workspaces;

namespace Notescrib.Notes.Features.Folders.Mappers;

public class FolderDetailsMapper : IMapper<Folder, FolderDetails>
{
    private readonly IMapper<Folder, FolderInfoBase> _baseMapper;

    public FolderDetailsMapper(IMapper<Folder, FolderInfoBase> baseMapper)
    {
        _baseMapper = baseMapper;
    }
    
    public FolderDetails Map(Folder item)
        => new()
        {
            Id = item.Id,
            Name = item.Name,
            Children = item.Children.Select(_baseMapper.Map).ToArray(),
            Created = item.Created,
            Updated = item.Updated,
            Notes = new List<NoteOverview>()
        };
}
