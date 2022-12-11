using System.Collections;
using Notescrib.Notes.Contracts;

namespace Notescrib.Notes.Utils.Tree;

public record BfsNode<T>(T Item, int Level);

public class BfsTreeEnumerable<T> : IEnumerable<BfsNode<T>>
    where T : IChildrenTree<IEnumerable<T>, T>
{
    private readonly T _root;

    public BfsTreeEnumerable(T root)
    {
        _root = root;
    }

    public IEnumerator<BfsNode<T>> GetEnumerator()
    {
        var level = 0;
        var queue = new Queue<T>();
        queue.Enqueue(_root);

        while (queue.Count > 0)
        {
            var levelSize = queue.Count;
            while (levelSize > 0)
            {
                var item = queue.Dequeue();
                levelSize--;
            
                yield return new(item, level);

                foreach (var child in item.ChildrenIds)
                {
                    queue.Enqueue(child);
                }
            }

            level++;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
