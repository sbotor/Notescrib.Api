using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Extensions;
using Notescrib.Api.Core.Exceptions;
using MongoDB.Driver.Linq;

namespace Notescrib.Api.Application.Tests;

internal class InMemoryPersistenceProvider<TEntity> : IPersistenceProvider<TEntity>
    where TEntity : EntityIdBase
{
    public ICollection<TEntity> Collection { get; }

    public InMemoryPersistenceProvider(ICollection<TEntity> collection)
    {
        Collection = collection;
    }

    public Task<string> AddAsync(TEntity entity)
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

        return Task.FromResult(entity.Id);
    }

    public async Task DeleteAsync(string id)
    {
        var found = await FindByIdAsync(id);

        if (found == null)
        {
            throw new NotFoundException();
        }

        Collection.Remove(found);
    }
    public async Task<bool> ExistsAsync(string id) => (await FindByIdAsync(id)) != null;
    public Task<TEntity?> FindByIdAsync(string id) => Task.FromResult(Collection.FirstOrDefault(x => x.Id == id));
    public async Task<IPagedList<TEntity>> FindPagedAsync(Expression<Func<TEntity, bool>> filter, IPaging paging, ISorting? sorting = null)
    {
        var result = await FindAsync(filter, sorting);
        return result.ToPagedList(paging);
    }

    public async Task UpdateAsync(TEntity entity)
    {
        var found = await FindByIdAsync(entity.Id ?? throw new InvalidOperationException("No entity ID."));
        if (found == null)
        {
            throw new NotFoundException("Item not found.");
        }

        Collection.Remove(found);
        Collection.Add(entity);
    }

    public Task<IReadOnlyCollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter, ISorting? sorting = null)
    {
        var output = Collection.AsQueryable().Where(filter);

        if (sorting != null && string.IsNullOrEmpty(sorting.OrderBy))
        {
            var directionString = sorting.Direction == Core.Enums.SortingDirection.Ascending
                ? "ASC"
                : "DESC";

            output = output.AsQueryable().OrderBy($"{sorting.OrderBy} {directionString}");
        }

        return Task.FromResult((IReadOnlyCollection<TEntity>)output.ToList());
    }
}
