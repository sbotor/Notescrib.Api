using System.Collections;

namespace Notescrib.Notes.Utils;

public class ReadOnlyCollectionWrapper<T> : IReadOnlyCollection<T>
{
    private readonly ICollection<T> _source;

    public ReadOnlyCollectionWrapper(ICollection<T> source)
    {
        _source = source;
    }

    public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _source.Count;
}
