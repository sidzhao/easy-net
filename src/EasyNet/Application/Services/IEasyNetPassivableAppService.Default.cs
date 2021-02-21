﻿using System;
using System.Threading.Tasks;
using EasyNet.Data;

namespace EasyNet.Application
{
    public abstract class EasyNetPassivableAppService<TEntity, TEntityDto, TGetAllInput> : EasyNetPassivableAppService<TEntity, TEntityDto, int, TGetAllInput, TEntityDto, TEntityDto>, IEasyNetPassivableAppService
        where TEntity : class, IEntity<int>, IPassivable
        where TEntityDto : IEntityDto<int>
    {
        protected EasyNetPassivableAppService(IServiceProvider serviceProvider, IRepository<TEntity, int> repository) : base(serviceProvider, repository)
        {
        }
    }

    public abstract class EasyNetPassivableAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput> : EasyNetPassivableAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TEntityDto, TEntityDto>
        where TEntity : class, IEntity<TPrimaryKey>, IPassivable
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
        protected EasyNetPassivableAppService(IServiceProvider serviceProvider, IRepository<TEntity, TPrimaryKey> repository) : base(serviceProvider, repository)
        {
        }
    }

    public abstract class EasyNetPassivableAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput> : EasyNetPassivableAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TEntityDto>
        where TEntity : class, IEntity<TPrimaryKey>, IPassivable
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
        protected EasyNetPassivableAppService(IServiceProvider serviceProvider, IRepository<TEntity, TPrimaryKey> repository) : base(serviceProvider, repository)
        {
        }
    }

    public abstract class EasyNetPassivableAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput> : EasyNetCrudAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput>, IEasyNetPassivableAppService<TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>, IPassivable
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
    {
        protected EasyNetPassivableAppService(IServiceProvider serviceProvider, IRepository<TEntity, TPrimaryKey> repository) : base(serviceProvider, repository)
        {
        }

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
