using System.Collections;

namespace Notescrib.Notes.Features.Workspaces.Utils;

public abstract class TreeEnumerable<TSource, TDest> : IEnumerable<TDest>
{
    private readonly IEnumerable<TSource> _rootItems;

    protected TreeEnumerable(IEnumerable<TSource> rootItems)
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

public abstract class TreeEnumerable<T> : TreeEnumerable<T, T>
{
    protected TreeEnumerable(IEnumerable<T> rootItems) : base(rootItems)
    {
    }

    protected override T GetDestinationItem(T source, int level) => source;
}
