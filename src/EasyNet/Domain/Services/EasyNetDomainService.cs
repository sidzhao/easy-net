using EasyNet.DependencyInjection;
using EasyNet.Domain.Entities;
using EasyNet.Domain.Repositories;

namespace EasyNet.Domain.Services
{
    public abstract class EasyNetDomainService : EasyNetServiceBase, IEasyNetDomainService
    {
        protected EasyNetDomainService(IIocResolver iocResolver) : base(iocResolver)
        {
        }
    }

    public abstract class EasyNetDomainService<TEntity> : EasyNetDomainService<TEntity, int>
        where TEntity : class, IEntity<int>
    {

        protected EasyNetDomainService(IIocResolver iocResolver, IRepository<TEntity, int> repository) : base(iocResolver, repository)
        {
        }
    }

    public abstract class EasyNetDomainService<TEntity, TPrimaryKey> : EasyNetDomainService
        where TEntity : class, IEntity<TPrimaryKey>
    {


        protected EasyNetDomainService(IIocResolver iocResolver, IRepository<TEntity, TPrimaryKey> repository) : base(iocResolver)
        {
            Repository = repository;
        }

        public IRepository<TEntity, TPrimaryKey> Repository { get; }
    }
}