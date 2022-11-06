using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Extensions;
using Notescrib.Api.Core.Exceptions;
using Notescrib.Api.Application.Common;

namespace Notescrib.Api.Application.Tests;

internal class InMemoryRepository<TEntity> : IRepository<TEntity>
    where TEntity : EntityIdBase
{
    public ICollection<TEntity> Collection { get; }

    public InMemoryRepository(ICollection<TEntity> collection)
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

    public async Task DeleteAsync(string id)
    {
        var found = await GetByIdAsync(id);

        if (found == null)
        {
            throw new NotFoundException();
        }

        Collection.Remove(found);
    }
    public async Task<bool> ExistsAsync(string id) => (await GetByIdAsync(id)) != null;
    public Task<TEntity?> GetByIdAsync(string id) => Task.FromResult(Collection.FirstOrDefault(x => x.Id == id));
    public async Task<IPagedList<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> filter, IPaging paging, ISorting? sorting = null)
    {
        var result = await GetAsync(filter, sorting);
        return result.ToPagedList(paging);
    }

    public async Task UpdateAsync(TEntity entity)
    {
        var found = await GetByIdAsync(entity.Id ?? throw new InvalidOperationException("No entity ID."));
        if (found == null)
        {
            throw new NotFoundException("Item not found.");
        }

        Collection.Remove(found);
        Collection.Add(entity);
    }

    public Task<IReadOnlyCollection<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter, ISorting? sorting = null)
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
