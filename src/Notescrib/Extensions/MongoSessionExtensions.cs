using System.Linq.Expressions;
using MongoDB.Driver;

namespace Notescrib.Notes.Extensions;

public static class MongoSessionExtensions
{
    public static Task SessionInsertOneAsync<T>(this IMongoCollection<T> collection,
        IClientSessionHandle? session,
        T item,
        CancellationToken cancellationToken = default)
        => session != null
            ? collection.InsertOneAsync(session, item, cancellationToken: cancellationToken)
            : collection.InsertOneAsync(item, cancellationToken: cancellationToken);

    public static IFindFluent<T, T> SessionFind<T>(this IMongoCollection<T> collection,
        IClientSessionHandle? session,
        Expression<Func<T, bool>> filter)
        => session != null
            ? collection.Find(session, filter)
            : collection.Find(filter);

    public static Task SessionUpdateOneAsync<T>(this IMongoCollection<T> collection,
        IClientSessionHandle? session,
        Expression<Func<T, bool>> filter,
        UpdateDefinition<T> update,
        CancellationToken cancellationToken = default)
        => session != null
            ? collection.UpdateOneAsync(session, filter, update, cancellationToken: cancellationToken)
            : collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

    public static Task SessionUpdateManyAsync<T>(this IMongoCollection<T> collection,
        IClientSessionHandle? session,
        Expression<Func<T, bool>> filter,
        UpdateDefinition<T> update,
        CancellationToken cancellationToken = default)
        => session != null
            ? collection.UpdateManyAsync(session, filter, update, cancellationToken: cancellationToken)
            : collection.UpdateManyAsync(filter, update, cancellationToken: cancellationToken);

    public static Task SessionDeleteOneAsync<T>(this IMongoCollection<T> collection,
        IClientSessionHandle? session,
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
        => session != null
            ? collection.DeleteOneAsync(session, filter, cancellationToken: cancellationToken)
            : collection.DeleteOneAsync(filter, cancellationToken: cancellationToken);

    public static Task SessionDeleteManyAsync<T>(this IMongoCollection<T> collection,
        IClientSessionHandle? session,
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default)
        => session != null
            ? collection.DeleteManyAsync(session, filter, cancellationToken: cancellationToken)
            : collection.DeleteManyAsync(filter, cancellationToken: cancellationToken);

    public static IAggregateFluent<T> SessionAggregate<T>(this IMongoCollection<T> collection,
        IClientSessionHandle? session,
        AggregateOptions? options = null)
        => session != null
            ? collection.Aggregate(session, options)
            : collection.Aggregate(options);
}
