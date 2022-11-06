using Notescrib.Api.Core.Contracts;

namespace Notescrib.Api.Core.Models;

public class TreeNode<T> : ITreeNode<T, TreeNode<T>>
{
    public T Item { get; }
    public IList<TreeNode<T>> Children { get; } = new List<TreeNode<T>>();

    public TreeNode(T item)
    {
        Item = item;
    }
}
