using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Helpers;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Core.Extensions;

public static class EnumerableExtensions
{
    public static PagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
    {
        var skipCount = PagingHelpers.CalculateSkipCount(pageNumber, pageSize);

        var totalCount = source.Count();
        var data = source.Skip(skipCount).Take(pageSize).ToList();

        return new(data, totalCount, pageNumber, pageSize);
    }

    public static PagedList<T> ToPagedList<T>(this IEnumerable<T> source, IPaging paging)
        => source.ToPagedList<T>(paging.PageNumber, paging.PageSize);
}
