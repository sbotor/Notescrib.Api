using Notescrib.Notes.Contracts;

namespace Notescrib.Notes.Utils.Tree;

public class Tree<T>
    where T : IChildrenTree<IEnumerable<T>, T>
{
    private readonly T _root;

    public Tree(T root)
    {
        _root = root;
    }
    
    public DfsNode<T>? VisitDepthFirst(Func<DfsNode<T>, bool> visitor)
        => EnumerateDepthFirst().FirstOrDefault(visitor.Invoke);

    public async ValueTask<DfsNode<T>?> VisitDepthFirstAsync(Func<DfsNode<T>, ValueTask<bool>> visitor)
    {
        foreach (var item in EnumerateDepthFirst())
        {
            if (await visitor.Invoke(item))
            {
                return item;
            }
        }

        return null;
    }
    
    public BfsNode<T>? VisitBreadthFirst(Func<BfsNode<T>, bool> visitor)
        => EnumerateBreadthFirst().FirstOrDefault(visitor.Invoke);

    public IEnumerable<DfsNode<T>> EnumerateDepthFirst()
        => new DfsTreeEnumerable<T>(_root);
    
    public IEnumerable<BfsNode<T>> EnumerateBreadthFirst()
        => new BfsTreeEnumerable<T>(_root);
}
