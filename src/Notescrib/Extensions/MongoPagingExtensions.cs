using MongoDB.Driver;
using Notescrib.Contracts;
using Notescrib.Models;
using Notescrib.Models.Enums;
using Notescrib.Utils;

namespace Notescrib.Extensions;

public static class MongoPagingExtensions
{
    private const string DataFacetName = "Data";
    private const string CountFacetName = "Count";

    public static Task<PagedList<T>> FindPagedAsync<T, TSort>(this IMongoCollection<T> source,
        IClientSessionHandle? session,
        IEnumerable<FilterDefinition<T>> filters,
        PagingSortingInfo<TSort> info,
        CancellationToken cancellationToken = default)
        where TSort : struct, Enum
        => source.ToPagedList<T, T, TSort>(session, filters, info, GetDataFacet<T, T, TSort>(info, null), cancellationToken);

    public static Task<PagedList<TOut>> FindPagedAsync<TIn, TOut, TSort>(this IMongoCollection<TIn> source,
        IClientSessionHandle? session,
        IEnumerable<FilterDefinition<TIn>> filters,
        PagingSortingInfo<TSort> info,
        ProjectionDefinition<TIn, TOut> projection,
        CancellationToken cancellationToken = default)
        where TSort : struct, Enum
        => source.ToPagedList<TIn, TOut, TSort>(session, filters, info, GetDataFacet(info, projection), cancellationToken);

    private static async Task<PagedList<TOut>> ToPagedList<TIn, TOut, TSort>(this IMongoCollection<TIn> source,
        IClientSessionHandle? session,
        IEnumerable<FilterDefinition<TIn>> filters,
        PagingSortingInfo<TSort> info,
        AggregateFacet<TIn> dataFacet,
        CancellationToken cancellationToken)
        where TSort : struct, Enum
    {
        var countFacet = GetCountFacet<TIn>();
        var query = source.GetQuery(session, filters)
            .Facet(countFacet, dataFacet);

        var result = await query
            .FirstAsync(cancellationToken);

        var count = result.Facets.First(x => x.Name == countFacet.Name)
            .Output<AggregateCountResult>()
            .FirstOrDefault()
            ?.Count ?? 0;

        var data = result.Facets.First(x => x.Name == DataFacetName)
            .Output<TOut>();

        return new PagedList<TOut>(data, info.Paging.Page, info.Paging.PageSize, (int)count);
    }

    private static IAggregateFluent<T> GetQuery<T>(this IMongoCollection<T> source,
        IClientSessionHandle? session,
        IEnumerable<FilterDefinition<T>> filters)
    {
        var query = source.SessionAggregate(session, new()
        {
            Collation = new("en", strength: CollationStrength.Secondary)
        });

        return filters.Aggregate(query, (current, filter) => current.Match(filter));
    }

    private static SortDefinition<T> GetSortDefinition<T, TSort>(Sorting<TSort> sorting, ISortingProvider<TSort> sortingProvider)
        where TSort : struct, Enum
    {
        var fieldDefinition = new StringFieldDefinition<T>(sortingProvider.GetSortName(sorting.OrderBy));

        return sorting.Direction == SortingDirection.Ascending
            ? Builders<T>.Sort.Ascending(fieldDefinition)
            : Builders<T>.Sort.Descending(fieldDefinition);
    }
    
    private static AggregateFacet<T> GetCountFacet<T>()
        => AggregateFacet.Create(CountFacetName,
            PipelineDefinition<T, AggregateCountResult>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Count<T>()
            }));

    private static AggregateFacet<TIn> GetDataFacet<TIn, TOut, TSort>(
        PagingSortingInfo<TSort> info,
        ProjectionDefinition<TIn, TOut>? projection)
        where TSort : struct, Enum
    {
        var stages = GetBaseDataPipelineStages<TIn, TSort>(info);

        if (projection != null)
        {
            stages.Add(PipelineStageDefinitionBuilder.Project(projection));
        }

        return AggregateFacet.Create(DataFacetName, PipelineDefinition<TIn, TOut>.Create(stages));
    }

    private static List<IPipelineStageDefinition> GetBaseDataPipelineStages<T, TSort>(
        PagingSortingInfo<TSort> info)
        where TSort : struct, Enum
        => new()
        {
            PipelineStageDefinitionBuilder.Sort(GetSortDefinition<T, TSort>(info.Sorting, info.SortingProvider)),
            PipelineStageDefinitionBuilder.Skip<T>(PagingHelper.CalculateSkipCount(info.Paging)),
            PipelineStageDefinitionBuilder.Limit<T>(info.Paging.PageSize)
        };
}
