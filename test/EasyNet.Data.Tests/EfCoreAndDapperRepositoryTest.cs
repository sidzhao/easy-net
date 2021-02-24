using EasyNet.CommonTests.Common;
using EasyNet.Data.Tests.Base;
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
                    });
                    x.UseDapper();
                })
                .AddSession<TestSession>()
                .AddCurrentDbConnectorProvider<TestCurrentDbConnectorProvider>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
