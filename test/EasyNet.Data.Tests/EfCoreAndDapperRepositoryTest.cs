using EasyNet.CommonTests.Common;
using EasyNet.CommonTests.Common.Entities;
using EasyNet.Dapper.Data;
using EasyNet.DependencyInjection;
using EasyNet.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyNet.Data.Tests
{
    public class EfCoreAndDapperRepositoryTest : DapperRepositoryTest
    {
        public EfCoreAndDapperRepositoryTest()
        {
            var services = new ServiceCollection();

            services
                .AddEasyNet(x =>
                {
                    x.Assemblies = new[] { this.GetType().Assembly };
                    x.UseEfCore<EfCoreContext>(options =>
                    {
                        options.UseSqlite(CreateInMemoryDatabase());
                    }).AsDefault<EfCoreContext>();
                    x.UseDapper().AsIDapperRepository(typeof(User).Assembly);
                })
                .AddSession<TestSession>()
                .AddCurrentDbConnectorProvider<TestCurrentDbConnectorProvider>();

            ServiceProvider = services.BuildServiceProvider();
        }

        public override IRepository<TEntity, TPrimaryKey> GetDapperRepository<TEntity, TPrimaryKey>()
        {
            return ServiceProvider.GetService<IDapperRepository<TEntity, TPrimaryKey>>();
        }
    }
}
