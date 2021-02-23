using EasyNet.Data.Tests.Base;
using EasyNet.Data.Tests.Core.Data;
using EasyNet.DependencyInjection;
using EasyNet.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EasyNet.Data.Tests
{
    public class DapperDataFilterTest : DataFilterTest
    {
        public DapperDataFilterTest()
        {
            var services = new ServiceCollection();

            services
                .AddEasyNet(x =>
                {
                    x.Assemblies = new[] { this.GetType().Assembly };
                    x.UseSqlLite("Filename=:memory:");
                    x.UseDapper();
                })
                .AddSession<TestSession>()
                .AddCurrentDbConnectorProvider<TestCurrentDbConnectorProvider>();

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
