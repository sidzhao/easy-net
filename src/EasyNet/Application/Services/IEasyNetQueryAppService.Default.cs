using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyNet.Application.Dto;
using EasyNet.Data.Entities;
using EasyNet.Data.Repositories;

namespace EasyNet.Application.Services
{
    public abstract class EasyNetQueryAppService<TEntity, TEntityDto, TGetAllInput> : EasyNetQueryAppService<TEntity, TEntityDto, int, TGetAllInput>, IEasyNetQueryAppService<TEntityDto, TGetAllInput>
        where TEntity : class, IEntity<int>
        where TEntityDto : IEntityDto<int>
    {
        protected EasyNetQueryAppService(IServiceProvider serviceProvider, IRepository<TEntity, int> repository) : base(serviceProvider, repository)
        {
        }
    }


    public abstract class EasyNetQueryAppService<TEntity, TEntityDto, TPrimaryKey, TGetAllInput> : EasyNetAppService, IEasyNetQueryAppService<TEntityDto, TPrimaryKey, TGetAllInput>
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
        protected EasyNetQueryAppService(IServiceProvider serviceProvider, IRepository<TEntity, TPrimaryKey> repository) : base(serviceProvider)
        {
            Repository = repository;
        }

        protected IRepository<TEntity, TPrimaryKey> Repository { get; }

        public async Task<TEntityDto> GetAsync(TPrimaryKey id)
        {
            var entity = await Repository.GetAsync(id);

            return MapToEntityDto(entity);
        }

        public async Task<List<TEntityDto>> GetAllAsync(TGetAllInput input)
        {
            var entities = await Repository.GetAllListAsync();

            return entities.Select(MapToEntityDto).ToList();
        }

        protected virtual TEntityDto MapToEntityDto(TEntity entity)
        {
            return ObjectMapper.Map<TEntityDto>(entity);
        }
    }
}
