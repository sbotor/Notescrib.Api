using System.Linq.Expressions;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Common;

public interface IPersistenceProvider<TEntity> where TEntity : EntityIdBase<string>
{
    Task<TEntity> AddAsync(TEntity entity);
    Task<bool> DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<TEntity?> FindByIdAsync(string id);
    Task<PagedList<TEntity>> FindPagedAsync(Expression<Func<TEntity, bool>> filter, IPaging paging, ISorting? sorting = null);
    Task UpdateAsync(TEntity entity);
}
