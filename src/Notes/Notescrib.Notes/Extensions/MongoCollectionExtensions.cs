using System.Linq.Expressions;
using MongoDB.Driver;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Models;
using Notescrib.Notes.Models.Enums;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Extensions;

public static class PagingExtensions
{
    public static async Task<PagedList<T>> FindPagedAsync<T, TSort>(this IMongoCollection<T> source,
        IEnumerable<Expression<Func<T, bool>>> filters,
        Paging paging,
        Sorting<TSort> sorting,
        ISortingProvider<TSort> sortingProvider)
        where TSort : struct, Enum
    {
        var countFacet = GetCountFacet<T>();
        var dataFacet = GetDataFacet<T, TSort>(paging, sorting, sortingProvider);

        var query = source.Aggregate();
        foreach (var filter in filters)
        {
            query = query.Match(filter);
        }
        
        var aggregation = await query
            .Facet(countFacet, dataFacet)
            .ToListAsync();

        var facets = aggregation.First().Facets;
        
        var count = facets.First(x => x.Name == countFacet.Name)
            .Output<AggregateCountResult>().FirstOrDefault()?.Count ?? 0;
        
        var data = facets.First(x => x.Name == dataFacet.Name)
            .Output<T>();

        return new PagedList<T>(data, paging.Page, paging.PageSize, (int)count);
    }

    public static Task<PagedList<T>> FindPagedAsync<T, TSort>(this IMongoCollection<T> source,
        Expression<Func<T, bool>> filter,
        Paging paging,
        Sorting<TSort> sorting,
        ISortingProvider<TSort> sortingProvider)
        where TSort : struct, Enum
        => source.FindPagedAsync(new[] { filter }, paging, sorting, sortingProvider);

    private static SortDefinition<T> GetSortDefinition<T, TSort>(Sorting<TSort> sorting, ISortingProvider<TSort> sortingProvider)
        where TSort : struct, Enum
    {
        var fieldDefinition = new StringFieldDefinition<T>(sortingProvider.GetSortName(sorting.OrderBy));

        return sorting.Direction == SortingDirection.Ascending
            ? Builders<T>.Sort.Ascending(fieldDefinition)
            : Builders<T>.Sort.Descending(fieldDefinition);
    }
    
    private static AggregateFacet<T> GetCountFacet<T>()
        => AggregateFacet.Create("count",
            PipelineDefinition<T, AggregateCountResult>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Count<T>()
            }));

    private static AggregateFacet<T, T> GetDataFacet<T, TSort>(
        Paging paging,
        Sorting<TSort> sorting,
        ISortingProvider<TSort> sortingProvider)
        where TSort : struct, Enum
        => AggregateFacet.Create("data",
            PipelineDefinition<T, T>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Sort(GetSortDefinition<T, TSort>(sorting, sortingProvider)),
                PipelineStageDefinitionBuilder.Skip<T>(PagingHelper.CalculateSkipCount(paging)),
                PipelineStageDefinitionBuilder.Limit<T>(paging.PageSize)
            }));
}
