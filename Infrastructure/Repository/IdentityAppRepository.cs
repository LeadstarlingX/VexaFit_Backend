﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Application.IRepository;
using Infrastructure.Context;
using Infrastructure.Extenstion;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class IdentityRepository<T>(IdentityAppDbContext context) : IIdentityAppRepository<T> where T : class
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly IdentityAppDbContext _context = context;
        /// <summary>
        /// The entities
        /// </summary>
        private readonly DbSet<T> _entities = context.Set<T>();

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <value>
        /// The table.
        /// </value>
        public IQueryable<T> Table => _entities;

        #region Find
        /// <summary>
        /// Finds the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="navigationProperties">The navigation properties.</param>
        /// <returns></returns>
        public IQueryable<T> Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> query = _entities;
            if (navigationProperties is not null)
                foreach (var navigationProperty in navigationProperties)
                    query = query.Include(navigationProperty);

            return query.Where(predicate);
        }

        /// <summary>
        /// Finds the asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="navigationProperties">The navigation properties.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> query = _entities;
            if (navigationProperties is not null)
                foreach (var navigationProperty in navigationProperties)
                    query = query.Include(navigationProperty);

            return await query.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Finds the with all include asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindWithAllIncludeAsync(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _entities.IncludeAll(_context);
            return await query.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Finds the with complex includes.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="includeExpression">The include expression.</param>
        /// <returns></returns>
        public IQueryable<T> FindWithComplexIncludes(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> includeExpression)
        {
            IQueryable<T> query = includeExpression(_entities);
            return query.Where(predicate);
        }

        #endregion

        #region Get
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="navigationProperties">The navigation properties.</param>
        /// <returns></returns>
        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> query = _entities;
            if (navigationProperties is not null)
                foreach (var navigationProperty in navigationProperties)
                    query = query.Include(navigationProperty);

            return query;
        }

        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <param name="navigationProperties">The navigation properties.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> query = _entities;
            if (navigationProperties is not null)
                foreach (var navigationProperty in navigationProperties)
                    query = query.Include(navigationProperty);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Gets all with all include asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllWithAllIncludeAsync()
        {
            IQueryable<T> query = _entities.IncludeAll(_context);
            return await query.ToListAsync();
        }

        #endregion

        #region CRUD
        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public async Task<T> UpdateAsync(T entity)
        {
            _entities.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Removes the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public async Task<T> RemoveAsync(T entity)
        {
            _entities.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Inserts the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public async Task<T> InsertAsync(T entity)
        {
            var newEntity = await _entities.AddAsync(entity);
            await _context.SaveChangesAsync();
            return newEntity.Entity;
        }

        #endregion
    }
}
