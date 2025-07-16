using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IRepository;
using Domain.Entities.Common;

namespace Application.IUnitOfWork
{
    public interface IAppUnitOfWork : IDisposable
    {
        IAppRepository<T> Repository<T>() where T : BaseEntity;
        Task<int> SaveChangesAsync();
    }
}
