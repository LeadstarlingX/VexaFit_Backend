using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IAppRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <param name="navigationPropertiesl">The navigation propertiesl.</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false, params Expression<Func<TEntity, object>>[] navigationPropertiesl);
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <param name="navigationProperties">The navigation properties.</param>
        /// <returns></returns>
        IQueryable<TEntity> GetAll(bool asNoTracking = false, params Expression<Func<TEntity, object>>[] navigationProperties);

        /// <summary>
        /// Gets all with all include.
        /// </summary>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <returns></returns>
        IQueryable<TEntity> GetAllWithAllInclude(bool asNoTracking = false);
        /// <summary>
        /// Gets all with all include asynchronous.
        /// </summary>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetAllWithAllIncludeAsync(bool asNoTracking = false);

        /// <summary>
        /// Finds the asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <param name="navigationProperties">The navigation properties.</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false, params Expression<Func<TEntity, object>>[] navigationProperties);
        /// <summary>
        /// Finds the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <param name="navigationProperties">The navigation properties.</param>
        /// <returns></returns>
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false, params Expression<Func<TEntity, object>>[] navigationProperties);
        /// <summary>
        /// Finds the with all include asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> FindWithAllIncludeAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);

        /// <summary>
        /// Inserts the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <returns></returns>
        Task<TEntity> InsertAsync(TEntity entity, bool asNoTracking = false);
        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TEntity entity, bool asNoTracking = false);
        /// <summary>
        /// Removes the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <returns></returns>
        Task<TEntity> RemoveAsync(TEntity entity, bool asNoTracking = false);
        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <value>
        /// The table.
        /// </value>
        IQueryable<TEntity> Table { get; }
        /// <summary>
        /// Finds the with complex includes.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="includeExpression">The include expression.</param>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <returns></returns>
        IQueryable<TEntity> FindWithComplexIncludes(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeExpression, bool asNoTracking = false);
    }
}
