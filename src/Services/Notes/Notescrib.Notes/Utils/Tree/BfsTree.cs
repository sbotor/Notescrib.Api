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
    
    public ICollection<T> Roots { get; }
    
    protected BfsTree(IEnumerable<T> roots)
    {
        Roots = roots.ToList();
    }
    
    protected BfsTree()
    {
        Roots = new List<T>();
    }

    protected IEnumerable<TreeNode<T>> AsNodeEnumerable()
        => new BfsTreeNodeEnumerable<T>(Roots);

    public IEnumerator<T> GetEnumerator() => new BfsTreeEnumerable<T>(Roots).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
