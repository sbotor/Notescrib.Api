using System.Collections;
using Notescrib.Notes.Contracts;

namespace Notescrib.Notes.Utils.Tree;

public record DfsNode<T>(T Item, DfsNode<T>? Parent);

public class DfsTreeEnumerable<T> : IEnumerable<DfsNode<T>>
    where T : IChildrenTree<IEnumerable<T>, T>
{
    private readonly IComparer<T>? _comparer;
    private readonly T _root;

    public DfsTreeEnumerable(T root, IComparer<T>? comparer = null)
    {
        _root = root;
        _comparer = comparer;
    }

    public IEnumerator<DfsNode<T>> GetEnumerator()
    {
        var stack = new Stack<DfsNode<T>>();

        stack.Push(new(_root, null));

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            yield return node;

            var helperList = node.Item.ChildrenIds.ToList();
            if (_comparer != null)
            {
                helperList.Sort(_comparer);
            }

            foreach (var child in helperList)
            {
                stack.Push(new(child, node));
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
