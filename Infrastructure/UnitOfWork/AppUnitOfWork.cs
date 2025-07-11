using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IRepository;
using Application.IUnitOfWork;
using Infrastructure.Context;
using Infrastructure.Repository;

namespace Infrastructure.UnitOfWork
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Application.IUnitOfWork.IAppUnitOfWork" />
    public class AppUnitOfWork : IAppUnitOfWork
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly ApplicationDbContext _context;
        /// <summary>
        /// The repositories
        /// </summary>
        private readonly Dictionary<Type, object> _repositories = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AppUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public AppUnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Repositories this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IAppRepository<T> Repository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return (IAppRepository<T>)_repositories[typeof(T)];
            }

            var repository = new AppRepository<T>(_context);
            _repositories.Add(typeof(T), repository);
            return repository;
        }

        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
