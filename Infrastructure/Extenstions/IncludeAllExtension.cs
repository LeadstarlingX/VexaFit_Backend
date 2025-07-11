using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extenstion
{
    /// <summary>
    /// 
    /// </summary>
    public static class IncludeAllExtension
    {
        /// <summary>
        /// Includes all.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static IQueryable<TEntity> IncludeAll<TEntity>(this IQueryable<TEntity> query, DbContext context) where TEntity : class
        {
            var entityType = context.Model.FindEntityType(typeof(TEntity));
            if (entityType == null)
                return query;

            var includedProperties = entityType.GetNavigations().ToList();
            foreach (var property in includedProperties)
            {
                query = query.Include(property.Name);
            }

            return query;
        }
    }
}
