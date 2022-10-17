using System.Linq.Expressions;
using MongoDB.Driver;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Infrastructure.MongoDb.Providers;

internal interface IMongoPersistenceProvider<TEntity> where TEntity : EntityIdBase<string>
{
    IMongoCollection<TEntity> Collection { get; }

    Task<TEntity> AddAsync(TEntity entity);
    Task<bool> DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<TEntity?> FindByIdAsync(string id);
    Task<PagedList<TEntity>> FindPagedAsync(Expression<Func<TEntity, bool>> filter, IPaging paging, ISorting? sorting = null);
    Task UpdateAsync(TEntity entity);
}
