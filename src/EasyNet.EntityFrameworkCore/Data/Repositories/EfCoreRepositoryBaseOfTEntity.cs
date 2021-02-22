using EasyNet.Data;

// ReSharper disable once CheckNamespace
namespace EasyNet.EntityFrameworkCore.Data
{
    public class EfCoreRepositoryBase<TDbContext, TEntity> : EfCoreRepositoryBase<TDbContext, TEntity, int>, IEfCoreRepository<TEntity>
         where TEntity : class, IEntity<int>
         where TDbContext : EasyNetDbContext
    {
        public EfCoreRepositoryBase(ICurrentDbConnectorProvider currentDbConnectorProvider)
            : base(currentDbConnectorProvider)
        {
        }
    }
}
