using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Core.Models;

public class FolderTree : FolderTreeBase<Folder>
{
    public FolderTree(IEnumerable<Folder> items) : base(items)
    {
    }
}
