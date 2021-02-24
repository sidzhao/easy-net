using EasyNet.CommonTests;
using EasyNet.Dapper.Repositories;
using EasyNet.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasyNet.Dapper.Tests
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
                    x.UseDapper();
                });

            services.BuildServiceProvider();

            // Assert
            AssertSpecifiedServiceTypeAndImplementationType<IQueryFilterExecuter, QueryFilterExecuter>(services, ServiceLifetime.Singleton);
        }
    }
}
