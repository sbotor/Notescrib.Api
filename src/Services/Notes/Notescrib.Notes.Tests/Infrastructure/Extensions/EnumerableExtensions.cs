using Notescrib.Notes.Models;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Tests.Infrastructure.Extensions;

public static class EnumerableExtensions
{
    public static PagedList<T> ToPagedList<T, TSort>(this IEnumerable<T> source, PagingSortingInfo<TSort> info)
        where TSort : struct, Enum
    {
        var skipCount = PagingHelper.CalculateSkipCount(info.Paging);

        var enumerated = source.ToArray();
        
        var totalCount = enumerated.Length;
        var data = enumerated.Skip(skipCount).Take(info.Paging.PageSize);

        return new PagedList<T>(data, info.Paging.Page, info.Paging.PageSize, totalCount);
    }
}
