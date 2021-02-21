using EasyNet.Data;

namespace EasyNet.EntityFrameworkCore.Domain.Repositories
{
    public class EfCoreRepositoryBase<TDbContext, TEntity> : EfCoreRepositoryBase<TDbContext, TEntity, int>, IEfCoreRepository<TEntity>
         where TEntity : class, IEntity<int>
         where TDbContext : EasyNetDbContext
    {
        public EfCoreRepositoryBase(IDbContextProvider dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
