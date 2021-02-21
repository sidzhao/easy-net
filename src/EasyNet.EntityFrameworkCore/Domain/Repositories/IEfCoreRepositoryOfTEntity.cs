using EasyNet.Data;

namespace EasyNet.EntityFrameworkCore.Domain.Repositories
{
    public interface IEfCoreRepository<TEntity> : IEfCoreRepository<TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
    }
}
