using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyNet.Data;
using EasyNet.Domain;
using EasyNet.Ioc;

namespace EasyNet.Application
{
    public abstract class EasyNetQueryAppServiceWithDomainService<TEntity, TEntityDto, TDomainService, TGetAllInput> : EasyNetQueryAppServiceWithDomainService<TEntity, TEntityDto, int, TDomainService, TGetAllInput>
        where TEntity : class, IEntity<int>
        where TEntityDto : IEntityDto<int>
        where TDomainService : EasyNetQueryDomainService<TEntity, int>
    {
        protected EasyNetQueryAppServiceWithDomainService(IIocResolver iocResolver, TDomainService domainService) : base(iocResolver, domainService)
        {
        }
    }

    public abstract class EasyNetQueryAppServiceWithDomainService<TEntity, TEntityDto, TPrimaryKey, TDomainService, TGetAllInput> : EasyNetAppService
        where TEntity : class, IEntity<TPrimaryKey>
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TDomainService : EasyNetQueryDomainService<TEntity, TPrimaryKey>
    {
        protected EasyNetQueryAppServiceWithDomainService(IIocResolver iocResolver, TDomainService domainService) : base(iocResolver)
        {
            DomainService = domainService;
        }

        protected TDomainService DomainService { get; }

        public async Task<TEntityDto> GetAsync(TPrimaryKey id)
        {
            var entity = await DomainService.GetByIdAsync(id);

            return MapToEntityDto(entity);
        }

        public async Task<List<TEntityDto>> GetAllAsync(TGetAllInput input)
        {
            var entities = await DomainService.GetAllAsync();

            return entities.Select(MapToEntityDto).ToList();
        }

        protected virtual TEntityDto MapToEntityDto(TEntity entity)
        {
            return ObjectMapper.Map<TEntityDto>(entity);
        }
    }
}
