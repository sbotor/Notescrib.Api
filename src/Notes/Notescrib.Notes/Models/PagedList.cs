using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Models;

public class PagedList<T>
{
    public IList<T> Data { get; }

    public int Page { get; }
    public int TotalPageCount { get; }
    public int PageSize { get; }
    public int TotalCount { get; }

    public PagedList(IEnumerable<T> data, int page, int pageSize, int totalCount)
    {
        Data = data.ToArray();

        Page = page;
        TotalCount = totalCount;
        PageSize = pageSize;

        TotalPageCount = PagingHelper.CalculatePageCount(totalCount, pageSize);
    }

    public PagedList<TDest> Map<TDest>(Func<T, TDest> mappingFunction)
        => new(
            Data.Select(mappingFunction),
            Page,
            PageSize,
            TotalCount);
}
