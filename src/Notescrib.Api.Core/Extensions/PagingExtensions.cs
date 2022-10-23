using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Helpers;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Core.Extensions;

public static class PagingExtensions
{
    public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        var skipCount = PagingHelpers.CalculateSkipCount(pageNumber, pageSize);

        var totalCount = source.Count();
        var data = source.Skip(skipCount).Take(pageSize).ToList();

        return new PagedList<T>(data, totalCount, pageNumber, pageSize);
    }

    public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, IPaging paging)
        => source.ToPagedList(paging.PageNumber, paging.PageSize);

    public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, IPaging paging)
        => source.AsQueryable().ToPagedList(paging.PageNumber, paging.PageSize);
}
