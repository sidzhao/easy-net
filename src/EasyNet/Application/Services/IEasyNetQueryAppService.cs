using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyNet.Application
{
    public interface IEasyNetQueryAppService<TEntityDto, in TGetAllInput> : IEasyNetQueryAppService<TEntityDto, int, TGetAllInput>
        where TEntityDto : IEntityDto<int>
    {
    }

    public interface IEasyNetQueryAppService<TEntityDto, in TPrimaryKey, in TGetAllInput> : IEasyNetAppService
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
        /// <summary>
        /// Get a object by id.
        /// </summary>
        /// <param name="id">The primary key of the object.</param>
        Task<TEntityDto> GetAsync(TPrimaryKey id);

        /// <summary>
        /// Gets all objects by condition.
        /// </summary>
        /// <param name="input">The filter condition values.</param>
        /// <returns></returns>
        Task<List<TEntityDto>> GetAllAsync(TGetAllInput input);
    }
}
