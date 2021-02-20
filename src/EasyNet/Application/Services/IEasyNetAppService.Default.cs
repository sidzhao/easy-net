using EasyNet.DependencyInjection;
using EasyNet.Domain.Entities;
using EasyNet.Domain.Repositories;
using EasyNet.Ioc;

namespace EasyNet.Application.Services
{
    /// <summary>
    /// This class can be used as a base class for application services.
    /// </summary>
    public abstract class EasyNetAppService : EasyNetServiceBase, IEasyNetAppService
    {
        protected EasyNetAppService(IIocResolver iocResolver) : base(iocResolver)
        {
        }
    }

    public abstract class EasyNetAppService<TEntity> : EasyNetAppService<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected EasyNetAppService(IIocResolver iocResolver, IRepository<TEntity, int> repository) : base(iocResolver, repository)
        {
        }
    }

    public abstract class EasyNetAppService<TEntity, TPrimaryKey> : EasyNetAppService
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected EasyNetAppService(IIocResolver iocResolver, IRepository<TEntity, TPrimaryKey> repository) : base(iocResolver)
        {
            Repository = repository;
        }

        protected IRepository<TEntity, TPrimaryKey> Repository { get; }
    }
}
