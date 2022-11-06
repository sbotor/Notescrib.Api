using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Helpers;

namespace Notescrib.Api.Core.Models;

public class PagedList<T> : IPagedList<T>
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

        TotalPageCount = PagingHelpers.CalculatePageCount(totalCount, pageSize);
    }

    public IPagedList<TOut> Map<TOut>(Func<T, TOut> mappingFunction)
        => new PagedList<TOut>(
            Data.Select(mappingFunction),
            PageNumber,
            PageSize,
            TotalCount);
}
