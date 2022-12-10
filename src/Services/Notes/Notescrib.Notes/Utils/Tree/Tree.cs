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
    
    public bool VisitDepthFirst(Func<DfsNode<T>, bool> visitor)
        => EnumerateDepthFirst().Any(visitor.Invoke);

    public async ValueTask<bool> VisitDepthFirstAsync(Func<DfsNode<T>, ValueTask<bool>> visitor)
    {
        foreach (var item in EnumerateDepthFirst())
        {
            if (await visitor.Invoke(item))
            {
                return true;
            }
        }

        return false;
    }
    
    public bool VisitBreadthFirst(Func<BfsNode<T>, bool> visitor)
        => EnumerateBreadthFirst().Any(visitor.Invoke);

    public IEnumerable<DfsNode<T>> EnumerateDepthFirst()
        => new DfsTreeEnumerable<T>(_root);
    
    public IEnumerable<BfsNode<T>> EnumerateBreadthFirst()
        => new BfsTreeEnumerable<T>(_root);
}
