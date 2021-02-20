
using EasyNet.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace EasyNet.EntityFrameworkCore.Domain.Uow
{
    public class DbContextCreator<TDbContext> : IDbContextCreator where TDbContext : EasyNetDbContext
    {
        protected readonly IIocResolver IocResolver;

        public DbContextCreator(IIocResolver iocResolver)
        {
            IocResolver = iocResolver;
        }

        public DbContext CreateDbContext()
        {
            return IocResolver.GetService<TDbContext>();
        }
    }
}
