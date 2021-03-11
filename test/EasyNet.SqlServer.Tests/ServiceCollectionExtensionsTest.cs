using EasyNet.CommonTests;
using EasyNet.Data;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.SqlServer.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasyNet.SqlServer.Tests
{
    public class ServiceCollectionExtensionsTest : DependencyInjectionTest
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
                    x.UseSqlServer("ConnectionString");
                });

            // Assert
            AssertSpecifiedServiceTypeAndImplementationType<IDbConnectorCreator, SqlServerConnectorCreator>(services, ServiceLifetime.Singleton);
        }
    }
}
