using Notescrib.Notes.Core.Contracts;
using Notescrib.Notes.Core.Helpers;
using Notescrib.Notes.Core.Models;

namespace Notescrib.Notes.Core.Extensions;

public static class PagingExtensions
{
    public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        var skipCount = PagingHelper.CalculateSkipCount(pageNumber, pageSize);

        var totalCount = source.Count();
        var data = source.Skip(skipCount).Take(pageSize).ToArray();

        return new PagedList<T>(data, pageNumber, pageSize, totalCount);
    }

    public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, IPaging paging)
        => source.ToPagedList(paging.PageNumber, paging.PageSize);

    public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, IPaging paging)
        => source.AsQueryable().ToPagedList(paging.PageNumber, paging.PageSize);
}
