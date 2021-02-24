using EasyNet.CommonTests.Common;
using EasyNet.Data.Tests.Base;
using EasyNet.DependencyInjection;
using EasyNet.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyNet.Data.Tests
{
    public class EfCoreDataFilterTest : DataFilterTest
    {
        public EfCoreDataFilterTest()
        {
            var services = new ServiceCollection();

            services
                .AddEasyNet(x =>
                {
                    x.UseEfCore<EfCoreContext>(options =>
                    {
                        options.UseSqlite(CreateInMemoryDatabase());
                    }).AsDefault<EfCoreContext>();
                })
                .AddSession<TestSession>()
                .AddCurrentDbConnectorProvider<TestCurrentDbConnectorProvider>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
