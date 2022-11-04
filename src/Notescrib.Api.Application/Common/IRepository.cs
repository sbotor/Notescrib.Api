using System.Linq.Expressions;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Common;

public interface IRepository<TEntity> where TEntity : EntityIdBase
{
    Task<string> AddAsync(TEntity entity);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<IReadOnlyCollection<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter, ISorting? sorting = null);
    Task<TEntity?> GetByIdAsync(string id);
    Task<IPagedList<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> filter, IPaging paging, ISorting? sorting = null);
    Task UpdateAsync(TEntity entity);
}
