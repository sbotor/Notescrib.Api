using System.Collections;

namespace Notescrib.Notes.Utils;

public abstract class TreeEnumerable<T> : IEnumerable<TreeNode<T>>
{
    private readonly IEnumerable<T> _rootItems;

    public TreeEnumerable(IEnumerable<T> rootItems)
    {
        _rootItems = rootItems;
    }
    
    public IEnumerator<TreeNode<T>> GetEnumerator()
    {
        var queue = new Queue<T>();
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
            
                yield return new TreeNode<T>(item, level);

                foreach (var child in GetChildren(item))
                {
                    queue.Enqueue(child);
                }
            }

            level++;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    protected abstract IEnumerable<T> GetChildren(T item);
}

public class TreeNode<T>
{
    public T Item { get; }
    public int Level { get; }
    public bool CanNestChildren => Level < Size.NestingLevel.Max;

    internal TreeNode(T item, int level)
    {
        Item = item;
        Level = level;
    }

    internal TreeNode(T item, TreeNode<T>? parent)
        : this(item, parent?.Level + 1 ?? 0)
    {
    }
}

public class TreeChildNode<T> : TreeNode<T>
{
    public TreeNode<T>? Parent { get; }
    
    internal TreeChildNode(T item, TreeNode<T>? parent) : base(item, parent)
    {
        Parent = parent;
    }
}
