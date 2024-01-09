using Notescrib.Contracts;
using Notescrib.Features.Folders.Models;

namespace Notescrib.Features.Folders.Mappers;

public class FolderDetailsMapper : IMapper<Folder, FolderDetails>
{
    private readonly IMapper<Folder, FolderOverview> _overviewMapper;

    public FolderDetailsMapper(
        IMapper<Folder, FolderOverview> overviewMapper)
    {
        _overviewMapper = overviewMapper;
    }
    
    public FolderDetails Map(Folder item)
        => new()
        {
            Id = item.Id,
            Name = item.Name,
            Children = item.Children.Select(_overviewMapper.Map).ToArray(),
            Created = item.Created,
            Updated = item.Updated
        };
}
