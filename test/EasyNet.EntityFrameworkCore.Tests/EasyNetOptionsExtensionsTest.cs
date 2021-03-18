using System.Linq;
using EasyNet.CommonTests;
using EasyNet.CommonTests.Common;
using EasyNet.Data;
using EasyNet.EntityFrameworkCore.Data;
using EasyNet.EntityFrameworkCore.Uow;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Sqlite.Infrastructure.Internal;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasyNet.EntityFrameworkCore.Tests
{
    public class EasyNetOptionsExtensionsTest : DependencyInjectionTest
    {
        [Fact]
        public void TestAddServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton(CommonTest.GetHostingEnvironment());

            // Act
            services
                .AddEasyNet(x =>
                {
                    x.UseEfCore<EfCoreContext>(options =>
                    {
                        options.UseSqlite("TestConnectionString");
                    });
                });
            
            var serviceProvider = services.BuildServiceProvider();
            var dbContextOptions = serviceProvider.GetService<DbContextOptions<EfCoreContext>>();
            var sqlServerOptions = dbContextOptions.Extensions.SingleOrDefault(p => p.GetType() == typeof(SqliteOptionsExtension));

            // Assert
            AssertSpecifiedServiceTypeAndImplementationType<EfCoreContext, EfCoreContext>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IDbConnectorCreator, EfCoreDbConnectorCreator<EfCoreContext>>(services, ServiceLifetime.Scoped);
            AssertSpecifiedServiceTypeAndImplementationType<IUnitOfWork, EfCoreUnitOfWork>(services, ServiceLifetime.Transient);

            Assert.NotNull(sqlServerOptions);
            Assert.Equal("TestConnectionString", ((RelationalOptionsExtension)sqlServerOptions).ConnectionString);
        }
    }
}
