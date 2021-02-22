using EasyNet.Data;

// ReSharper disable once CheckNamespace
namespace EasyNet.EntityFrameworkCore.Data
{
    public interface IEfCoreRepository<TEntity> : IEfCoreRepository<TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
    }
}
