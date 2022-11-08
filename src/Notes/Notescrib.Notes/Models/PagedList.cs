using Notescrib.Notes.Application.Utils;

namespace Notescrib.Notes.Application.Models;

public class PagedList<T>
{
    public IList<T> Data { get; }

    public int PageNumber { get; }
    public int TotalPageCount { get; }
    public int PageSize { get; }
    public int TotalCount { get; }

    public PagedList(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
    {
        Data = data.ToArray();

        PageNumber = pageNumber;
        TotalCount = totalCount;
        PageSize = pageSize;

        TotalPageCount = PagingHelper.CalculatePageCount(totalCount, pageSize);
    }

    public PagedList<TDest> Map<TDest>(Func<T, TDest> mappingFunction)
        => new(
            Data.Select(mappingFunction),
            PageNumber,
            PageSize,
            TotalCount);
}
