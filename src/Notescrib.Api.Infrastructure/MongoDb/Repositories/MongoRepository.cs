using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Enums;
using Notescrib.Api.Core.Exceptions;
using Notescrib.Api.Core.Extensions;
using Notescrib.Api.Core.Helpers;
using Notescrib.Api.Infrastructure.MongoDb.Providers;

namespace Notescrib.Api.Infrastructure.MongoDb.Repositories;

internal class MongoRepository<TEntity> : IMongoRepository<TEntity>
    where TEntity : EntityIdBase
{
    private readonly IDateTimeProvider _dateTimeProvider;

    protected IMongoCollection<TEntity> Collection { get; }

    public MongoRepository(IMongoCollectionProvider collectionProvider, IDateTimeProvider dateTimeProvider)
    {
        Collection = collectionProvider.GetCollection<TEntity>();
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<string> AddAsync(TEntity entity)
    {
        if (entity is ICreatedTimestamp created)
        {
            created.Created = _dateTimeProvider.UtcNow;
        }

        if (entity is IUpdatedTimestamp updated)
        {
            updated.Updated = _dateTimeProvider.UtcNow;
        }

        entity.Id = string.IsNullOrEmpty(entity.Id)
            ? ObjectId.GenerateNewId().ToString()
            : entity.Id;

        await Collection.InsertOneAsync(entity);

        return entity.Id;
    }

    public async Task DeleteAsync(string id)
    {
        var found = await Collection.FindOneAndDeleteAsync(d => d.Id == id);
        if (found != null)
        {
            throw new NotFoundException("The resource was not found.");
        }
    }

    public async Task UpdateAsync(TEntity entity)
    {
        if (string.IsNullOrEmpty(entity.Id))
        {
            throw new InvalidOperationException("Cannot update an entity with an empty Id.");
        }

        if (entity is IUpdatedTimestamp updated)
        {
            updated.Updated = _dateTimeProvider.UtcNow;
        }

        await Collection.FindOneAndReplaceAsync(d => d.Id == entity.Id, entity);
    }

    public async Task<TEntity?> GetByIdAsync(string id)
    {
        var result = await Collection.FindAsync(d => d.Id == id);
        return result.SingleOrDefault();
    }

    public async Task<IReadOnlyCollection<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter, ISorting? sorting = null)
    {
        IAsyncCursor<TEntity> result;
        if (sorting == null)
        {
            result = await Collection.FindAsync(filter);
        }
        else
        {
            var options = new FindOptions<TEntity>
            {
                Sort = GetSortDefinition(sorting)
            };

            result = await Collection.FindAsync(filter, options);
        }

        return await result.ToListAsync();
    }

    public async Task<IPagedList<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> filter, IPaging paging, ISorting? sorting = null)
    {
        var filterDefinition = new ExpressionFilterDefinition<TEntity>(filter);
        return await FindPagedAsync(filterDefinition, paging, sorting);
    }

    public async Task<IPagedList<TEntity>> FindPagedAsync(FilterDefinition<TEntity> filter, IPaging paging, ISorting? sorting = null)
    {
        var options = new FindOptions<TEntity>
        {
            Skip = PagingHelpers.CalculateSkipCount(paging.PageNumber, paging.PageSize),
            Limit = paging.PageSize
        };

        if (sorting != null && string.IsNullOrEmpty(sorting.OrderBy))
        {
            options.Sort = GetSortDefinition(sorting);
        }

        var result = await Collection.FindAsync(filter, options);
        var data = await result.ToListAsync();

        return data.ToPagedList(paging);
    }

    public async Task<bool> ExistsAsync(string id)
        => await GetByIdAsync(id) != null;

    private static SortDefinition<TEntity> GetSortDefinition(ISorting sorting)
    {
        var fieldDefinition = new StringFieldDefinition<TEntity>(sorting.OrderBy);

        return sorting.Direction == SortingDirection.Ascending
            ? Builders<TEntity>.Sort.Ascending(fieldDefinition)
            : Builders<TEntity>.Sort.Descending(fieldDefinition);
    }
}
