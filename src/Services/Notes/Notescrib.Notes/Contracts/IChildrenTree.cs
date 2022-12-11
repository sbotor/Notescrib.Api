namespace Notescrib.Notes.Contracts;

public interface IChildrenTree<out TEnumerable, TItem>
    where TEnumerable : IEnumerable<TItem>
    where TItem : IChildrenTree<TEnumerable, TItem>
{
    TEnumerable ChildrenIds { get; }
}
