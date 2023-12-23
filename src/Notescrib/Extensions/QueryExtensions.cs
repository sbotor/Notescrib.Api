using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Notescrib.Models;

namespace Notescrib.Extensions;

public static class QueryExtensions
{
    public static async Task<PagedList<TResult>> Paginate<TEntity, TResult>(this IQueryable<TEntity> query,
        Paging paging,
        Func<TEntity, TResult> mapper,
        CancellationToken cancellationToken = default)
    {
        var (data, count) = await query.PaginateRaw(paging, cancellationToken);

        return new(data.Select(mapper), paging.Page, paging.PageSize, count);
    }

    public static async Task<(TEntity[], int)> PaginateRaw<TEntity>(this IQueryable<TEntity> query, 
        Paging paging,
        CancellationToken cancellationToken = default)
    {
        var skipCount = (paging.Page - 1) * paging.PageSize;
        query = query.Skip(skipCount).Take(paging.PageSize);

        var count = await query.CountAsync(cancellationToken);
        var data = await query.ToArrayAsync(cancellationToken);

        return (data, count);
    }

    public static IQueryable<T> Where<T>(this IQueryable<T> query,
        Expression<Func<T, bool>> predicate,
        bool condition)
        => condition ? query.Where(predicate) : query;
}
