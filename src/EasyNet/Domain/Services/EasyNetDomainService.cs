using System;
using EasyNet.Data;

namespace EasyNet.Domain
{
    public abstract class EasyNetDomainService : EasyNetServiceBase, IEasyNetDomainService
    {
        protected EasyNetDomainService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }

    public abstract class EasyNetDomainService<TEntity> : EasyNetDomainService<TEntity, int>
        where TEntity : class, IEntity<int>
    {

        protected EasyNetDomainService(IServiceProvider serviceProvider, IRepository<TEntity, int> repository) : base(serviceProvider, repository)
        {
        }
    }

    public abstract class EasyNetDomainService<TEntity, TPrimaryKey> : EasyNetDomainService
        where TEntity : class, IEntity<TPrimaryKey>
    {


        protected EasyNetDomainService(IServiceProvider serviceProvider, IRepository<TEntity, TPrimaryKey> repository) : base(serviceProvider)
        {
            Repository = repository;
        }

        public IRepository<TEntity, TPrimaryKey> Repository { get; }
    }
}