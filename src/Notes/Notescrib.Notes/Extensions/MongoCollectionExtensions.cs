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
        Expression<Func<T, bool>> filter,
        Paging paging,
        Sorting<TSort> sorting,
        ISortingProvider<TSort> sortingProvider)
        where TSort : struct, Enum
    {
        var countFacet = AggregateFacet.Create("count",
            PipelineDefinition<T, AggregateCountResult>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Count<T>()
            }));

        var dataFacet = AggregateFacet.Create("data",
            PipelineDefinition<T, T>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Sort(GetSortDefinition<T, TSort>(sorting, sortingProvider)),
                PipelineStageDefinitionBuilder.Skip<T>(PagingHelper.CalculateSkipCount(paging)),
                PipelineStageDefinitionBuilder.Limit<T>(paging.PageSize)
            }));

        var aggregation = await source.Aggregate()
            .Match(filter)
            .Facet(countFacet, dataFacet)
            .ToListAsync();

        var facets = aggregation.First().Facets;
        
        var count = facets.First(x => x.Name == countFacet.Name)
            .Output<AggregateCountResult>().FirstOrDefault()?.Count ?? 0;
        
        var data = facets.First(x => x.Name == dataFacet.Name)
            .Output<T>();

        return new PagedList<T>(data, paging.PageNumber, paging.PageSize, (int)count);
    }
    
    private static SortDefinition<T> GetSortDefinition<T, TSort>(Sorting<TSort> sorting, ISortingProvider<TSort> sortingProvider)
        where TSort : struct, Enum
    {
        var fieldDefinition = new StringFieldDefinition<T>(sortingProvider.GetSortName(sorting.OrderBy));

        return sorting.Direction == SortingDirection.Ascending
            ? Builders<T>.Sort.Ascending(fieldDefinition)
            : Builders<T>.Sort.Descending(fieldDefinition);
    }
}
