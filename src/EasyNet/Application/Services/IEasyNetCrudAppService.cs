using System.Threading.Tasks;

namespace EasyNet.Application
{
    public interface IEasyNetCrudAppService<TEntityDto> : IEasyNetCrudAppService<TEntityDto, int, TEntityDto, TEntityDto>
        where TEntityDto : IEntityDto<int>
    {
    }

    public interface IEasyNetCrudAppService<TEntityDto, in TPrimaryKey> : IEasyNetCrudAppService<TEntityDto, TPrimaryKey, TEntityDto, TEntityDto>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
    }

    public interface IEasyNetCrudAppService<TEntityDto, in TPrimaryKey, in TCreateInput> : IEasyNetCrudAppService<TEntityDto, TPrimaryKey, TCreateInput, TEntityDto>
        where TEntityDto : IEntityDto<TPrimaryKey>
    {
    }

    public interface IEasyNetCrudAppService<TEntityDto, in TPrimaryKey, in TCreateInput, in TUpdateInput> : IEasyNetAppService
        where TEntityDto : IEntityDto<TPrimaryKey>
        where TUpdateInput : IEntityDto<TPrimaryKey>
    {
        /// <summary>
        /// Create a object.
        /// </summary>
        /// <param name="input">The input values for creating a object.</param>
        Task<TEntityDto> CreateAsync(TCreateInput input);

        /// <summary>
        /// Update a object.
        /// </summary>
        /// <param name="input">The input values for updating a object.</param>
        Task<TEntityDto> UpdateAsync(TUpdateInput input);

        /// <summary>
        /// Delete a object by id.
        /// </summary>
        /// <param name="id">The primary key of the object.</param>
        Task DeleteAsync(TPrimaryKey id);
    }
}
