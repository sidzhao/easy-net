using System;
using System.Threading.Tasks;
using EasyNet.Data;

namespace EasyNet.Domain
{
    public abstract class EasyNetCrudDomainService<TEntity> : EasyNetCrudDomainService<TEntity, int>, IEasyNetCrudDomainService<TEntity>
        where TEntity : class, IEntity<int>
    {
        protected EasyNetCrudDomainService(IServiceProvider serviceProvider, IRepository<TEntity, int> repository) : base(serviceProvider, repository)
        {
        }
    }

    public abstract class EasyNetCrudDomainService<TEntity, TPrimaryKey> : EasyNetQueryDomainService<TEntity, TPrimaryKey>, IEasyNetCrudDomainService<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected EasyNetCrudDomainService(IServiceProvider serviceProvider, IRepository<TEntity, TPrimaryKey> repository) : base(serviceProvider, repository)
        {
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));

            await Repository.InsertAndGetIdAsync(entity);

            return entity;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            Check.NotNull(entity, nameof(entity));

            return Repository.UpdateAsync(entity);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task DeleteAsync(TPrimaryKey id)
        {
            return Repository.DeleteAsync(id);
        }
    }
}