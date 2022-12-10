using Notescrib.Notes.Contracts;
using Notescrib.Notes.Utils.Tree;

namespace Notescrib.Notes.Extensions;

public static class TreeEnumerableExtensions
{
    public static BfsTreeEnumerable<T> ToBfsEnumerable<T>(this T root)
        where T : IChildrenTree<IEnumerable<T>, T>
        => new(root);
    
    public static DfsTreeEnumerable<T> ToDfsEnumerable<T>(this T root)
        where T : IChildrenTree<IEnumerable<T>, T>
        => new(root);
}
