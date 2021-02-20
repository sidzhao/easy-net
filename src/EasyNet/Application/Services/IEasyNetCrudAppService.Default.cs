using System.Threading.Tasks;
using EasyNet.Application.Dto;
using EasyNet.Domain.Entities;
using EasyNet.Domain.Repositories;
using EasyNet.Ioc;

namespace EasyNet.Application.Services
{
    public abstract class EasyNetCrudAppService<TEntity, TEntityDto, TGetAllInput> : EasyNetCrudAppService<TEntity, TEntityDto, int, TGetAllInput, TEntityDto, TEntityDto>, IEasyNetCrudAppService<TEntityDto>
        where TEntity : class, IEntity<int>
        where TEntityDto : IEntityDto<int>
    {
        protected EasyNetCrudAppService(IIocResolver iocResolver, IRepository<TEntity, int> repository) : base(iocResolver, repository)
        {
        }
    }

    public abstract class EasyNetCrudAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput> : EasyNetCrudAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TEntityDto, TEntityDto>, IEasyNetCrudAppService<TEntityDto, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
        protected EasyNetCrudAppService(IIocResolver iocResolver, IRepository<TEntity, TPrimaryKey> repository) : base(iocResolver, repository)
        {
        }
    }

    public abstract class EasyNetCrudAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput> : EasyNetCrudAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TEntityDto>, IEasyNetCrudAppService<TEntityDto, TPrimaryKey, TCreateInput>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
        protected EasyNetCrudAppService(IIocResolver iocResolver, IRepository<TEntity, TPrimaryKey> repository) : base(iocResolver, repository)
        {
        }
    }

    public abstract class EasyNetCrudAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput, TCreateInput, TUpdateInput> : EasyNetQueryAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput>, IEasyNetCrudAppService<TEntityDto, TPrimaryKey, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
    {
        protected EasyNetCrudAppService(IIocResolver iocResolver, IRepository<TEntity, TPrimaryKey> repository) : base(iocResolver, repository)
        {
        }

        public virtual async Task<TEntityDto> CreateAsync(TCreateInput input)
        {
            var entity = MapCreateInputToEntity(input);

            await Repository.InsertAndGetIdAsync(entity);

            return MapToEntityDto(entity);
        }

        protected virtual TEntity MapCreateInputToEntity(TCreateInput input)
        {
            var entity = ObjectMapper.Map<TEntity>(input);

            return entity;
        }
        public virtual async Task<TEntityDto> UpdateAsync(TUpdateInput input)
        {
            var entity = await Repository.GetAsync(input.Id);

            MapUpdateInputToEntity(input, entity);

            await Repository.UpdateAsync(entity);

            return MapToEntityDto(entity);
        }
        protected virtual void MapUpdateInputToEntity(TUpdateInput input, TEntity entity)
        {
            ObjectMapper.Map(input, entity);
        }

        public virtual Task DeleteAsync(TPrimaryKey id)
        {
            return Repository.DeleteAsync(id);
        }
    }
}
