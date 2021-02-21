using System.Threading.Tasks;
using EasyNet.Data;

namespace EasyNet.Domain
{
    public interface IEasyNetCrudDomainService<TEntity> : IEasyNetCrudDomainService<TEntity, int>
        where TEntity : IEntity<int>
    {
    }

    public interface IEasyNetCrudDomainService<TEntity, in TPrimaryKey> : IEasyNetDomainService
        where TEntity : IEntity<TPrimaryKey>
    {
        Task<TEntity> CreateAsync(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task DeleteAsync(TPrimaryKey id);
    }
}
