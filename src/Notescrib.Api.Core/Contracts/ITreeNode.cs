namespace Notescrib.Api.Core.Contracts;

public interface ITreeNode<T, TNode> where TNode : ITreeNode<T, TNode>
{
    int NestingLevel { get; }
    T Item { get; }
    IList<TNode> Children { get; }
}
