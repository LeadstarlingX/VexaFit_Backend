﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IRepository;

namespace Application.IUnitOfWork
{
    public interface IAppUnitOfWork : IDisposable
    {
        IAppRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync();
    }
}
