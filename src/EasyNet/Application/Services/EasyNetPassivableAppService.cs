﻿using System.Threading.Tasks;
using EasyNet.DependencyInjection;
using EasyNet.Domain.Entities;
using EasyNet.Domain.Repositories;
using EasyNet.Dto;

namespace EasyNet.Application.Services
{
    public abstract class EasyNetPassivableAppService<TEntity, TEntityDto, TGetAllInput> : EasyNetPassivableAppService<TEntity, TEntityDto, int, TGetAllInput, TEntityDto, TEntityDto>, IEasyNetPassivableAppService
        where TEntity : class, IEntity<int>, IPassivable
        where TEntityDto : IEntityDto<int>
    {
        protected EasyNetPassivableAppService(IIocResolver iocResolver, IRepository<TEntity, int> repository) : base(iocResolver, repository)
        {
        }
    }

    public abstract class EasyNetPassivableAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput> : EasyNetPassivableAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TEntityDto, TEntityDto>
        where TEntity : class, IEntity<TPrimaryKey>, IPassivable
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
        protected EasyNetPassivableAppService(IIocResolver iocResolver, IRepository<TEntity, TPrimaryKey> repository) : base(iocResolver, repository)
        {
        }
    }

    public abstract class EasyNetPassivableAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput> : EasyNetPassivableAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TEntityDto>
        where TEntity : class, IEntity<TPrimaryKey>, IPassivable
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
        protected EasyNetPassivableAppService(IIocResolver iocResolver, IRepository<TEntity, TPrimaryKey> repository) : base(iocResolver, repository)
        {
        }
    }

    public abstract class EasyNetPassivableAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput> : EasyNetCrudAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput>, IEasyNetPassivableAppService<TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>, IPassivable
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
    {
        protected EasyNetPassivableAppService(IIocResolver iocResolver, IRepository<TEntity, TPrimaryKey> repository) : base(iocResolver, repository)
        {
        }

        /// <summary>
        /// Archive
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task ArchiveAsync(TPrimaryKey id)
        {
            var entity = await Repository.GetAsync(id);

            if (entity is IPassivable passivable)
            {
                if (entity.IsActive) return;

                passivable.IsActive = true;

                await Repository.UpdateAsync(entity);

                return;
            }

            throw new EasyNetException($"The {entity.GetType().AssemblyQualifiedName} is not inherit from {typeof(IPassivable)}.");
        }

        /// <summary>
        /// Activate
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task ActivateAsync(TPrimaryKey id)
        {
            var entity = await Repository.GetAsync(id);

            if (entity is IPassivable passivable)
            {
                if (!entity.IsActive) return;

                passivable.IsActive = false;

                await Repository.UpdateAsync(entity);

                return;
            }

            throw new EasyNetException($"The {entity.GetType().AssemblyQualifiedName} is not inherit from {typeof(IPassivable)}.");
        }
    }
}
