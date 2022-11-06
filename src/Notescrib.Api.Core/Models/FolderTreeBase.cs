using System.Collections;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Helpers;

namespace Notescrib.Api.Core.Models;

public abstract class FolderTreeBase<T, TSource, TNode> : IEnumerable<TNode>
    where T : IEntityId
    where TSource : IFolderStructure
    where TNode : class, ITreeNode<T, TNode>
{
    protected Func<TSource, T> Mapping { get; }

    public IList<TNode> Items { get; }

    protected FolderTreeBase(IEnumerable<TSource> items, Func<TSource, T> mapping)
    {
        Mapping = mapping;
        Items = CreateTree(items.ToList());
    }

    protected IList<TNode> CreateTree(IReadOnlyCollection<TSource> items)
    {
        if (!items.Any())
        {
            return new List<TNode>();
        }

        var itemsToAdd = items.ToList();
        var roots = new List<TNode>();
        var parentLookup = new Dictionary<string, TNode>();
        var i = 0;

        while (itemsToAdd.Count > 0)
        {
            i %= itemsToAdd.Count;
            var item = itemsToAdd[i];

            if (item.ParentId != null)
            {
                if (parentLookup.TryGetValue(item.ParentId, out var parent))
                {
                    var node = CreateNode(parent, item);
                    parent.Children.Add(node);

                    itemsToAdd.RemoveAt(i);

                    parentLookup.Add(node.Item.Id, node);

                    continue;
                }

                i++;
            }
            else
            {
                var node = CreateNode(null, item);
                roots.Add(node);

                itemsToAdd.RemoveAt(i);

                parentLookup.Add(node.Item.Id, node);
            }
        }

        return roots;
    }

    protected abstract TNode CreateNode(TNode? parent, TSource item);

    public IEnumerator<TNode> GetEnumerator()
    {
        var queue = new Queue<TNode>();
        foreach (var item in Items)
        {
            queue.Enqueue(item);
        }

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            yield return node;

            foreach (var child in node.Children)
            {
                queue.Enqueue(child);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public abstract class FolderTreeBase<T, TSource> : FolderTreeBase<T, TSource, FolderTreeBase<T, TSource>.Node>
    where T : IEntityId
    where TSource : IFolderStructure
{
    protected FolderTreeBase(IEnumerable<TSource> items, Func<TSource, T> mapping)
        : base(items, mapping)
    {
    }

    protected override Node CreateNode(Node? parent, TSource item)
        => new(Mapping.Invoke(item), parent);

    public class Node : ITreeNode<T, Node>
    {
        public int NestingLevel { get; }
        public Node? Parent { get; }
        public T Item { get; }
        public IList<Node> Children { get; } = new List<Node>();

        public bool CanNestChildren => NestingLevel < Size.NestingLevel.Max;

        public Node(T item, Node? parent)
        {
            Item = item;
            Parent = parent;
            NestingLevel = (Parent?.NestingLevel + 1) ?? 0;
        }

        public Node? FindAncestor(Predicate<Node> predicate)
        {
            var node = this;
            var i = Size.NestingLevel.Max;

            while (i >= 0)
            {
                if (node == null)
                {
                    return null;
                }

                if (predicate.Invoke(node))
                {
                    return node;
                }

                node = node.Parent;
                i--;
            }

            return null;
        }
    }
}
