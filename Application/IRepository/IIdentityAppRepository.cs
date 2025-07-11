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
    public interface IIdentityAppRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <param name="navigationPropertiesl">The navigation propertiesl.</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] navigationPropertiesl);
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="navigationProperties">The navigation properties.</param>
        /// <returns></returns>
        IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] navigationProperties);
        /// <summary>
        /// Gets all with all include asynchronous.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetAllWithAllIncludeAsync();

        /// <summary>
        /// Finds the asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="navigationProperties">The navigation properties.</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties);
        /// <summary>
        /// Finds the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="navigationProperties">The navigation properties.</param>
        /// <returns></returns>
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] navigationProperties);
        /// <summary>
        /// Finds the with all include asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> FindWithAllIncludeAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Inserts the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        Task<TEntity> InsertAsync(TEntity entity);
        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TEntity entity);
        /// <summary>
        /// Removes the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        Task<TEntity> RemoveAsync(TEntity entity);
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
        /// <returns></returns>
        IQueryable<TEntity> FindWithComplexIncludes(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeExpression);
    }
}
