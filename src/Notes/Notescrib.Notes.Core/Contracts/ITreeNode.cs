namespace Notescrib.Notes.Core.Contracts;

public interface ITreeNode<T, TNode> where TNode : ITreeNode<T, TNode>
{
    T Item { get; }
    IList<TNode> Children { get; }
}
