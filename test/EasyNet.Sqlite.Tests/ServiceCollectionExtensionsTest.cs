﻿using EasyNet.CommonTests;
using EasyNet.Data;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.Sqlite.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasyNet.Sqlite.Tests
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
                    x.UseSqlite("Filename=:memory:");
                });

            // Assert
            AssertSpecifiedServiceTypeAndImplementationType<IDbConnectorCreator, SqliteConnectorCreator>(services, ServiceLifetime.Singleton);
        }
    }
}
