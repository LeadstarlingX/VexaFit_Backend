using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Common;

namespace Application.IRepository
{
    public interface IAppRepository<TEntity> where TEntity : BaseEntity
    {

        #region Get
        Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false, params Expression<Func<TEntity, object>>[] navigationPropertiesl);
        IQueryable<TEntity> GetAll(bool asNoTracking = false, params Expression<Func<TEntity, object>>[] navigationProperties);

        IQueryable<TEntity> GetAllWithAllInclude(bool asNoTracking = false);
        Task<IEnumerable<TEntity>> GetAllWithAllIncludeAsync(bool asNoTracking = false);

        #endregion

        #region Find
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false, params Expression<Func<TEntity, object>>[] navigationProperties);
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false, params Expression<Func<TEntity, object>>[] navigationProperties);
        Task<IEnumerable<TEntity>> FindWithAllIncludeAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);
        IQueryable<TEntity> FindWithComplexIncludes(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeExpression, bool asNoTracking = false);


        #endregion

        #region CRUD

        Task<TEntity> InsertAsync(TEntity entity, bool asNoTracking = false);
        Task BulkInsertAsync(IEnumerable<TEntity> entities);


        Task<TEntity> UpdateAsync(TEntity entity, bool asNoTracking = false);
        Task BulkUpdateAsync(IEnumerable<TEntity> entities);

        Task<TEntity> RemoveAsync(TEntity entity, bool asNoTracking = false);
        Task BulkRemoveAsync(IEnumerable<int> ids);
        IQueryable<TEntity> Table { get; }

        #endregion

    }
}
