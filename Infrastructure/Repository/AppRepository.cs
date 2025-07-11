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
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Application.IRepository.IAppRepository&lt;T&gt;" />
    public class AppRepository<T>(ApplicationDbContext context) : IAppRepository<T> where T : class
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly ApplicationDbContext _context = context;
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
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <param name="navigationProperties">The navigation properties.</param>
        /// <returns></returns>
        public IQueryable<T> Find(
            Expression<Func<T, bool>> predicate,
            bool asNoTracking = false,
            params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> query = _entities;

            if (asNoTracking)
                query = query.AsNoTracking();

            if (navigationProperties is not null && navigationProperties.Any())
            {
                foreach (var navigationProperty in navigationProperties)
                    query = query.Include(navigationProperty);
            }

            return query.Where(predicate);
        }

        /// <summary>
        /// Finds the asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <param name="navigationProperties">The navigation properties.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            bool asNoTracking = false,
            params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> query = _entities;

            if (asNoTracking)
                query = query.AsNoTracking();

            if (navigationProperties is not null && navigationProperties.Any())
            {
                foreach (var navigationProperty in navigationProperties)
                    query = query.Include(navigationProperty);
            }

            return await query.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Finds the with all include asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindWithAllIncludeAsync(
            Expression<Func<T, bool>> predicate,
            bool asNoTracking = false)
        {
            IQueryable<T> query = _entities.IncludeAll(_context);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Finds the with complex includes.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="includeExpression">The include expression.</param>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <returns></returns>
        public IQueryable<T> FindWithComplexIncludes(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>> includeExpression,
            bool asNoTracking = false)
        {
            IQueryable<T> query = includeExpression(_entities);

            if (asNoTracking)
                query = query.AsNoTracking();

            return query.Where(predicate);
        }

        #endregion

        #region Get

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <param name="navigationProperties">The navigation properties.</param>
        /// <returns></returns>
        public IQueryable<T> GetAll(
            bool asNoTracking = false,
            params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> query = _entities;

            if (asNoTracking)
                query = query.AsNoTracking();

            if (navigationProperties is not null && navigationProperties.Any())
            {
                foreach (var navigationProperty in navigationProperties)
                    query = query.Include(navigationProperty);
            }

            return query;
        }

        /// <summary>
        /// Gets all asynchronous.
        /// </summary>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <param name="navigationProperties">The navigation properties.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllAsync(
            bool asNoTracking = false,
            params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> query = _entities;

            if (asNoTracking)
                query = query.AsNoTracking();

            if (navigationProperties is not null && navigationProperties.Any())
            {
                foreach (var navigationProperty in navigationProperties)
                    query = query.Include(navigationProperty);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Gets all with all include.
        /// </summary>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <returns></returns>
        public IQueryable<T> GetAllWithAllInclude(
            bool asNoTracking = false)
        {
            IQueryable<T> query = _entities.IncludeAll(_context);

            if (asNoTracking)
                query = query.AsNoTracking();

            return query;
        }

        /// <summary>
        /// Gets all with all include asynchronous.
        /// </summary>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllWithAllIncludeAsync(
            bool asNoTracking = false)
        {
            IQueryable<T> query = _entities.IncludeAll(_context);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        #endregion

        #region CRUD

        /// <summary>
        /// Inserts the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <returns></returns>
        public async Task<T> InsertAsync(T entity, bool asNoTracking = false)
        {
            var newEntity = await _entities.AddAsync(entity);
            await _context.SaveChangesAsync();
            return newEntity.Entity;
        }

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <returns></returns>
        public async Task<T> UpdateAsync(T entity, bool asNoTracking = false)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Removes the asynchronous.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="asNoTracking">if set to <c>true</c> [as no tracking].</param>
        /// <returns></returns>
        public async Task<T> RemoveAsync(T entity, bool asNoTracking = false)
        {
            _entities.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        #endregion

    }
}
