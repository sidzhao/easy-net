using EasyNet.CommonTests;
using EasyNet.CommonTests.Common.Entities;
using EasyNet.Dapper.Data.Repositories;
using EasyNet.Data.Repositories;
using EasyNet.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasyNet.Dapper.Tests
{
    public class EasyNetRepositoryBuilderExtensionsTest : DependencyInjectionTest
    {
        [Fact]
        public void TestAddRepositoryAsDefault()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton(CommonTest.GetHostingEnvironment());

            // Act
            services
                .AddEasyNet(x =>
                {
                    x.UseDapper().AsDefault(typeof(User).Assembly);
                });

            services.BuildServiceProvider();

            // Assert
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<User, long>, DapperRepositoryBase<User, long>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<Role, int>, DapperRepositoryBase<Role, int>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<Role>, DapperRepositoryBase<Role>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<User, long>, DapperRepositoryBase<User, long>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<Role, int>, DapperRepositoryBase<Role, int>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<Role>, DapperRepositoryBase<Role>>(services, ServiceLifetime.Transient);
        }

        [Fact]
        public void TestAddRepositoryAsIDapperRepository()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton(CommonTest.GetHostingEnvironment());

            // Act
            services
                .AddEasyNet(x =>
                {
                    x.UseDapper().AsIDapperRepository(typeof(User).Assembly);
                });

            services.BuildServiceProvider();

            // Assert
            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<User, long>, DapperRepositoryBase<User, long>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<Role, int>, DapperRepositoryBase<Role, int>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<Role>, DapperRepositoryBase<Role>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<User, long>, DapperRepositoryBase<User, long>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<Role, int>, DapperRepositoryBase<Role, int>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<Role>, DapperRepositoryBase<Role>>(services, ServiceLifetime.Transient);
        }

        [Fact]
        public void TestAddRepositoryAsBoth()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton(CommonTest.GetHostingEnvironment());

            // Act
            services
                .AddEasyNet(x =>
                {
                    var repositoryBuilder = x.UseDapper();
                    repositoryBuilder.AsDefault(typeof(User).Assembly);
                    repositoryBuilder.AsIDapperRepository(typeof(User).Assembly);
                });

            services.BuildServiceProvider();

            // Assert
            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<User, long>, DapperRepositoryBase<User, long>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<Role, int>, DapperRepositoryBase<Role, int>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<Role>, DapperRepositoryBase<Role>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<User, long>, DapperRepositoryBase<User, long>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<Role, int>, DapperRepositoryBase<Role, int>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<Role>, DapperRepositoryBase<Role>>(services, ServiceLifetime.Transient);

            AssertSpecifiedServiceTypeAndImplementationType<IRepository<User, long>, DapperRepositoryBase<User, long>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<Role, int>, DapperRepositoryBase<Role, int>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<Role>, DapperRepositoryBase<Role>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<User, long>, DapperRepositoryBase<User, long>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<Role, int>, DapperRepositoryBase<Role, int>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<Role>, DapperRepositoryBase<Role>>(services, ServiceLifetime.Transient);
        }
    }
}
