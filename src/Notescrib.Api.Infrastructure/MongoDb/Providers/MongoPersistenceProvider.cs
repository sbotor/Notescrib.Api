using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Enums;
using Notescrib.Api.Core.Exceptions;
using Notescrib.Api.Core.Extensions;
using Notescrib.Api.Core.Helpers;

namespace Notescrib.Api.Infrastructure.MongoDb.Providers;

internal class MongoPersistenceProvider<TEntity> : IPersistenceProvider<TEntity>
    where TEntity : EntityIdBase
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMongoCollection<TEntity> _collection;

    public MongoPersistenceProvider(IMongoCollectionProvider collectionProvider, IDateTimeProvider dateTimeProvider)
    {
        _collection = collectionProvider.GetCollection<TEntity>();
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

        await _collection.InsertOneAsync(entity);

        return entity.Id;
    }

    public async Task DeleteAsync(string id)
    {
        var found = await _collection.FindOneAndDeleteAsync(d => d.Id == id);
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

        await _collection.FindOneAndReplaceAsync(d => d.Id == entity.Id, entity);
    }

    public async Task<TEntity?> FindByIdAsync(string id)
    {
        var result = await _collection.FindAsync(d => d.Id == id);
        return result.SingleOrDefault();
    }

    public async Task<IPagedList<TEntity>> FindPagedAsync(Expression<Func<TEntity, bool>> filter, IPaging paging, ISorting? sorting = null)
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
            var fieldDefinition = new StringFieldDefinition<TEntity>(sorting.OrderBy);

            var sortDefinition = sorting.Direction == SortingDirection.Ascending
                ? Builders<TEntity>.Sort.Ascending(fieldDefinition)
                : Builders<TEntity>.Sort.Descending(fieldDefinition);

            options.Sort = sortDefinition;
        }

        var result = await _collection.FindAsync(filter, options);
        var data = await result.ToListAsync();

        return data.ToPagedList(paging);
    }

    public async Task<bool> ExistsAsync(string id)
        => (await FindByIdAsync(id)) != null;
}
