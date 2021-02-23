using EasyNet.CommonTests;
using EasyNet.Data;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.SqlLite.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasyNet.SqlLite.Tests
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
                    x.UseSqlLite("Filename=:memory:");
                });

            // Assert
            AssertSpecifiedServiceTypeAndImplementationType<IDbConnectorCreator, SqlLiteConnectorCreator>(services, ServiceLifetime.Singleton);
        }
    }
}
