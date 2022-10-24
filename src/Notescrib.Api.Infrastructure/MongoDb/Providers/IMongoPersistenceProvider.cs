using MongoDB.Driver;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Infrastructure.MongoDb.Providers;

internal interface IMongoPersistenceProvider<TEntity> : IPersistenceProvider<TEntity>
    where TEntity : EntityIdBase
{
    Task<IPagedList<TEntity>> FindPagedAsync(FilterDefinition<TEntity> filter, IPaging paging, ISorting? sorting = null);
}
