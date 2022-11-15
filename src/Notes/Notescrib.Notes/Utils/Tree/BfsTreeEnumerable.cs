using System.Collections;
using Notescrib.Notes.Contracts;

namespace Notescrib.Notes.Utils.Tree;

public abstract class BfsTreeEnumerable<TSource, TDest> : IEnumerable<TDest>
{
    private readonly IEnumerable<TSource> _rootItems;

    protected BfsTreeEnumerable(IEnumerable<TSource> rootItems)
    {
        _rootItems = rootItems;
    }
    
    public IEnumerator<TDest> GetEnumerator()
    {
        var queue = new Queue<TSource>();
        foreach (var item in _rootItems)
        {
            queue.Enqueue(item);
        }

        var level = 0;

        while (queue.Count > 0)
        {
            var levelSize = queue.Count;
            while (levelSize > 0)
            {
                var item = queue.Dequeue();
                levelSize--;
            
                yield return GetDestinationItem(item, level);

                foreach (var child in GetChildren(item))
                {
                    queue.Enqueue(child);
                }
            }

            level++;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    protected abstract TDest GetDestinationItem(TSource source, int level);
    protected abstract IEnumerable<TSource> GetChildren(TSource item);
}

public class BfsTreeEnumerable<T> : BfsTreeEnumerable<T, T>
    where T : IChildrenTree<IEnumerable<T>, T>
{
    public BfsTreeEnumerable(IEnumerable<T> rootItems) : base(rootItems)
    {
    }

    protected override T GetDestinationItem(T source, int level) => source;
    protected override IEnumerable<T> GetChildren(T item) => item.Children;
}

public class BfsTreeNodeEnumerable<T> : BfsTreeEnumerable<T, TreeNode<T>>
    where T : IChildrenTree<IEnumerable<T>, T>
{
    public BfsTreeNodeEnumerable(IEnumerable<T> rootItems)
        : base(rootItems)
    {
    }

    protected override TreeNode<T> GetDestinationItem(T source, int level) => new(source, level);
    protected override IEnumerable<T> GetChildren(T item) => item.Children;
}
