using Microsoft.EntityFrameworkCore;

namespace Notescrib.Features.Folders.Extensions;

public static class FolderQueryExtensions
{
    public static Task<Folder?> GetFolderOrRootAsync(this IQueryable<Folder> query,
        string ownerId,
        Guid? id,
        CancellationToken cancellationToken = default)
    {
        query = query.Where(x => x.OwnerId == ownerId);
        
        query = id.HasValue
            ? query.Where(x => x.Id == id)
            : query.Where(x => x.ParentId == null);

        return query.FirstOrDefaultAsync(cancellationToken);
    }
}
