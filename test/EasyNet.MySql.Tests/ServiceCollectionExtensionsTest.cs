using EasyNet.CommonTests;
using EasyNet.Data;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.MySql.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasyNet.MySql.Tests
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
                    x.UseMySql("ConnectionString");
                });

            // Assert
            AssertSpecifiedServiceTypeAndImplementationType<IDbConnectorCreator, MySqlConnectorCreator>(services, ServiceLifetime.Singleton);
        }
    }
}
