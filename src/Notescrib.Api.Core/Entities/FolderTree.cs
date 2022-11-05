using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Core.Entities;

public class FolderTree : FolderTreeBase<Folder>
{
    public FolderTree(IEnumerable<Folder> items) : base(items)
    {
    }
}
