using System.Collections;
using Notescrib.Notes.Contracts;

namespace Notescrib.Notes.Utils.Tree;

public abstract class BfsTree<T> : IEnumerable<T>
    where T : IChildrenCollectionTree<T>
{
    private int? _count;
    public int Count
    {
        get => _count ??= this.Count();
        protected set => _count = value;
    }
    
    public T Root { get; }
    
    protected BfsTree(T root)
    {
        Root = root;
    }

    public TreeChildNode<T>? FindWithParent(Func<T, bool> predicate)
    {
        var items = this.ToArray();

        foreach (var item in items)
        {
            var found = item.Children.SingleOrDefault(predicate);
            if (found == null)
            {
                continue;
            }
            
            return new(found, item);
        }

        return null;
    }

    protected IEnumerable<TreeNode<T>> AsNodeEnumerable()
        => new BfsTreeNodeEnumerable<T>(Root);

    public IEnumerator<T> GetEnumerator() => new BfsTreeEnumerable<T>(Root).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
