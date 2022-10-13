using MongoDB.Driver;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Infrastructure.MongoDb.Providers;

internal interface IMongoPersistenceProvider<TEntity> where TEntity : EntityIdBase<string>
{
    IMongoCollection<TEntity> Collection { get; }

    Task<TEntity> AddAsync(TEntity entity);
    Task<bool> DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<TEntity?> FindByIdAsync(string id);
    Task UpdateAsync(TEntity entity);
}
