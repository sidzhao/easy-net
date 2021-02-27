using System;
using EasyNet.Data.Entities;
using EasyNet.Data.Repositories;

namespace EasyNet.Application.Services
{
    /// <summary>
    /// This class can be used as a base class for application services.
    /// </summary>
    public abstract class EasyNetAppService : EasyNetServiceBase, IEasyNetAppService
    {
        protected EasyNetAppService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }

    public abstract class EasyNetAppService<TEntity> : EasyNetAppService<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected EasyNetAppService(IServiceProvider serviceProvider, IRepository<TEntity, int> repository) : base(serviceProvider, repository)
        {
        }
    }

    public abstract class EasyNetAppService<TEntity, TPrimaryKey> : EasyNetAppService
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected EasyNetAppService(IServiceProvider serviceProvider, IRepository<TEntity, TPrimaryKey> repository) : base(serviceProvider)
        {
            Repository = repository;
        }

        protected IRepository<TEntity, TPrimaryKey> Repository { get; }
    }
}
