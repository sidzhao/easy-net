using System.Linq;
using System.Threading.Tasks;
using EasyNet.CommonTests.Common;
using EasyNet.CommonTests.Common.Entities;
using EasyNet.Data.Tests.Base;
using EasyNet.DependencyInjection;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasyNet.Data.Tests
{
    public class EfCoreRepositoryTest : RepositoryTest
    {
        public EfCoreRepositoryTest()
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
