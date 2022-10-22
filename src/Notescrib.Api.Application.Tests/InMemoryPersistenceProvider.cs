using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;
using Notescrib.Api.Core.Extensions;

namespace Notescrib.Api.Application.Tests;

internal class InMemoryPersistenceProvider<TEntity> : IPersistenceProvider<TEntity>
    where TEntity : EntityIdBase
{
    public ICollection<TEntity> Collection { get; }

    public InMemoryPersistenceProvider(ICollection<TEntity> collection)
    {
        Collection = collection;
    }

    public Task<TEntity> AddAsync(TEntity entity)
    {
        if (entity is ICreatedTimestamp created)
        {
            created.Created = DateTime.Now;
        }

        if (entity is IUpdatedTimestamp updated)
        {
            updated.Updated = DateTime.Now;
        }

        entity.Id = Guid.NewGuid().ToString();
        Collection.Add(entity);

        return Task.FromResult(entity);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var success = false;
        var found = await FindByIdAsync(id);

        if (found != null)
        {
            Collection.Remove(found);
            success = true;
        }

        return success;
    }
    public async Task<bool> ExistsAsync(string id) => (await FindByIdAsync(id)) != null;
    public Task<TEntity?> FindByIdAsync(string id) => Task.FromResult(Collection.FirstOrDefault(x => x.Id == id));
    public Task<PagedList<TEntity>> FindPagedAsync(Expression<Func<TEntity, bool>> filter, IPaging paging, ISorting? sorting = null)
    {
        var output = Collection.AsQueryable().Where(filter);

        if (sorting != null && string.IsNullOrEmpty(sorting.OrderBy))
        {
            var directionString = sorting.Direction == Core.Enums.SortingDirection.Ascending
                ? "ASC"
                : "DESC";

            output = output.AsQueryable().OrderBy($"{sorting.OrderBy} {directionString}");
        }

        return Task.FromResult(output.ToPagedList(paging));
    }

    public async Task UpdateAsync(TEntity entity)
    {
        var found = await FindByIdAsync(entity.Id ?? throw new InvalidOperationException("No entity ID."));
        if (found == null)
        {
            throw new InvalidOperationException("Item not found.");
        }

        Collection.Remove(found);
        Collection.Add(entity);
    }
}
