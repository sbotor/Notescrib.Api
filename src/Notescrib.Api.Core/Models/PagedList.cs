namespace Notescrib.Api.Core.Models;

public class PagedList<T>
{
    public IReadOnlyCollection<T> Data { get; }

    public int PageNumber { get; }
    public int TotalPageCount { get; }
    public int PageSize { get; }
    public int TotalCount { get; }

    public PagedList(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
    {
        Data = data.ToList();

        PageNumber = pageNumber;
        TotalCount = totalCount;
        PageSize = pageSize;

        TotalPageCount = (totalCount + pageSize - 1) / pageSize;
    }

    public PagedList<TOut> Map<TOut>(Func<T, TOut> mappingFunction)
        => new(Data.Select(x => mappingFunction.Invoke(x)), PageNumber, PageSize, TotalCount);
}
