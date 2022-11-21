namespace Notescrib.Notes.Contracts;

public interface IChildrenCollectionTree<T> : IChildrenTree<ICollection<T>, T>
    where T : IChildrenCollectionTree<T>
{
}

public interface IChildrenTree<out TEnumerable, TItem>
    where TEnumerable : IEnumerable<TItem>
    where TItem : IChildrenTree<TEnumerable, TItem>
{
    TEnumerable Children { get; }
}
