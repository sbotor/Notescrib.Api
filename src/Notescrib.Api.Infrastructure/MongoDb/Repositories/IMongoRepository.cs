using MongoDB.Driver;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Infrastructure.MongoDb.Repositories;

internal interface IMongoRepository<TEntity> : IRepository<TEntity>
    where TEntity : EntityIdBase
{
    Task<IPagedList<TEntity>> FindPagedAsync(FilterDefinition<TEntity> filter, IPaging paging, ISorting? sorting = null);
}
