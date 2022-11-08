using System.Linq.Expressions;
using Notescrib.Notes.Core.Contracts;
using Notescrib.Notes.Core.Entities;

namespace Notescrib.Notes.Application.Common.Contracts;

public interface IRepository<TEntity> where TEntity : EntityIdBase
{
    Task<TEntity> AddAsync(TEntity entity);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<IReadOnlyCollection<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter, ISorting? sorting = null);
    Task<TEntity?> GetByIdAsync(string id);
    Task<IPagedList<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> filter, IPaging paging, ISorting? sorting = null);
    Task UpdateAsync(TEntity entity);
}
