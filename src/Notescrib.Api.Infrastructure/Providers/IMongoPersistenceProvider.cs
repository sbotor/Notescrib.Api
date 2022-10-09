using MongoDB.Driver;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Infrastructure.Services;

internal interface IMongoPersistenceProvider<TEntity> where TEntity : IdEntityBase<string>
{
    IMongoCollection<TEntity> Collection { get; }

    Task<TEntity> AddAsync(TEntity entity);
    Task<bool> DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<TEntity?> FindByIdAsync(string id);
    Task UpdateAsync(TEntity entity);
}
