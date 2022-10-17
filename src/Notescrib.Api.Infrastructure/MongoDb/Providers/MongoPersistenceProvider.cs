using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Enums;
using Notescrib.Api.Core.Extensions;
using Notescrib.Api.Core.Helpers;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Infrastructure.MongoDb.Providers;

internal class MongoPersistenceProvider<TEntity> : IMongoPersistenceProvider<TEntity>
    where TEntity : EntityIdBase<string>
{
    public IMongoCollection<TEntity> Collection { get; }

    public MongoPersistenceProvider(IMongoCollectionProvider collectionProvider)
    {
        Collection = collectionProvider.GetCollection<TEntity>();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        if (!string.IsNullOrEmpty(entity.Id))
        {
            throw new InvalidOperationException("Cannot add an entity with an existing Id.");
        }

        entity.Id = ObjectId.GenerateNewId().ToString();
        await Collection.InsertOneAsync(entity);

        return entity;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var found = await Collection.FindOneAndDeleteAsync(d => d.Id == id);
        return found != null;
    }

    public async Task UpdateAsync(TEntity entity)
    {
        if (string.IsNullOrEmpty(entity.Id))
        {
            throw new InvalidOperationException("Cannot update an entity with an empty Id.");
        }

        await Collection.FindOneAndReplaceAsync(d => d.Id == entity.Id, entity);
    }

    public async Task<TEntity?> FindByIdAsync(string id)
    {
        var result = await Collection.FindAsync(d => d.Id == id);
        return result.SingleOrDefault();
    }

    public async Task<PagedList<TEntity>> FindPagedAsync(Expression<Func<TEntity, bool>> filter, IPaging paging, ISorting? sorting = null)
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

        var result = await Collection.FindAsync(filter, options);
        var data = await result.ToListAsync();

        return data.ToPagedList(paging);
    }

    public async Task<bool> ExistsAsync(string id)
        => await FindByIdAsync(id) != null;
}
