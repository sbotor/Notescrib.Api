using Notescrib.Api.Core.Contracts;

namespace Notescrib.Api.Core.Models;

public abstract class FolderTreeBase<T, TSource>
    where T : IEntityId
    where TSource : IFolderStructure
{
    protected Func<TSource, T> Mapping { get; }

    public IList<Node> Items { get; } = new List<Node>();

    protected FolderTreeBase(IEnumerable<TSource> items, Func<TSource, T> mapping)
    {
        Mapping = mapping;
        Items = CreateTree(items.ToList());
    }

    public bool CanMoveFolder(string id, string targetId)
    {
        var node = FindNode(id) ?? throw new InvalidOperationException();
        var target = FindNode(id) ?? throw new InvalidOperationException();

        return node.Level >= target.Level;
    }

    protected virtual IList<Node> CreateTree(IReadOnlyCollection<TSource> items)
    {
        if (!items.Any())
        {
            return new List<Node>();
        }

        var itemsToAdd = items.ToList();
        var roots = new List<Node>();
        var parentLookup = new Dictionary<string, Node>();
        var i = 0;

        while (itemsToAdd.Count > 0)
        {
            i %= itemsToAdd.Count;
            var item = itemsToAdd[i];

            if (item.ParentId != null)
            {
                if (parentLookup.TryGetValue(item.ParentId, out var parent))
                {
                    var node = new Node(Mapping.Invoke(item), parent);
                    parent.Children.Add(node);
                    itemsToAdd.RemoveAt(i);

                    parentLookup.Add(node.Item.Id, node);

                    continue;
                }

                i++;
            }
            else
            {
                var node = new Node(Mapping.Invoke(item), null);
                roots.Add(node);
                itemsToAdd.RemoveAt(i);

                parentLookup.Add(node.Item.Id, node);
            }
        }

        return roots;
    }

    private Node? FindNode(Predicate<Node> predicate)
    {
        var queue = new Queue<Node>();
        foreach (var item in Items)
        {
            queue.Enqueue(item);
        }

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            if (predicate.Invoke(node))
            {
                return node;
            }

            foreach (var child in node.Children)
            {
                queue.Enqueue(child);
            }
        }

        return null;
    }

    private Node? FindNode(string id) => FindNode(x => x.Item.Id == id);

    public class Node
    {
        internal int Level { get; }
        internal Node? Parent { get; }

        public T Item { get; }

        public IList<Node> Children { get; } = new List<Node>();

        internal Node(T item, Node? parent)
        {
            Item = item;
            Parent = parent;
            Level = (Parent?.Level + 1) ?? 0;
        }
    }
}

public abstract class FolderTreeBase<T> : FolderTreeBase<T, T>
    where T : IFolderStructure
{
    protected FolderTreeBase(IEnumerable<T> items) : base(items, x => x)
    {
    }
}
