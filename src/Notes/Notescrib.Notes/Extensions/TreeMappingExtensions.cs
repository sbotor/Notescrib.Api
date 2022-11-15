using Notescrib.Notes.Contracts;

namespace Notescrib.Notes.Extensions;

public static class TreeMappingExtensions
{
    public static IEnumerable<TDest> MapTree<TSource, TDest>(this IEnumerable<TSource> source, Func<TSource, TDest> mapping)
        where TSource : IChildrenTree<IEnumerable<TSource>, TSource>
        where TDest : IChildrenCollectionTree<TDest>
    {
        var stack = new Stack<TSource>();
        var mappedStack = new Stack<TDest>();
        var output = new List<TDest>();

        foreach (var item in source)
        {
            var mapped = mapping.Invoke(item);
            output.Add(mapped);

            stack.Push(item);
            mappedStack.Push(mapped);
        }

        while (stack.Count > 0)
        {
            var original = stack.Pop();
            var mapped = mappedStack.Pop();

            foreach (var child in original.Children)
            {
                var mappedChild = mapping.Invoke(child);
                mapped.Children.Add(mappedChild);

                stack.Push(child);
                mappedStack.Push(mappedChild);
            }
        }

        return output;
    }
}
