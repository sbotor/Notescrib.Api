using Notescrib.Notes.Core.Contracts;

namespace Notescrib.Notes.Core.Models;

public class TreeNode<T> : ITreeNode<T, TreeNode<T>>
{
    public T Item { get; }
    public IList<TreeNode<T>> Children { get; } = new List<TreeNode<T>>();

    public TreeNode(T item)
    {
        Item = item;
    }
}
