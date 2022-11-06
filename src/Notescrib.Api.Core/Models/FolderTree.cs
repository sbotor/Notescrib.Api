using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Core.Models;

public class FolderTree : FolderTreeBase<Folder, Folder>
{
    public FolderTree(IEnumerable<Folder> items) : base(items, x => x)
    {
    }
}

public class FolderTree<T, TSource> : FolderTreeBase<T, TSource, TreeNode<T>>
    where T : IEntityId
    where TSource : IFolderStructure
{
    public FolderTree(IEnumerable<TSource> items, Func<TSource, T> mapping)
        : base(items, mapping)
    {
    }

    protected override TreeNode<T> CreateNode(TreeNode<T>? parent, TSource item)
        => new(Mapping.Invoke(item));
}
