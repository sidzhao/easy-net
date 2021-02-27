using EasyNet.CommonTests;
using EasyNet.CommonTests.Common;
using EasyNet.CommonTests.Common.Entities;
using EasyNet.Dapper.Data.Repositories;
using EasyNet.Data.Repositories;
using EasyNet.EntityFrameworkCore.Data.Repositories;
using EasyNet.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasyNet.Data.Tests
{
    public class EasyNetRepositoryBuilderExtensionsTest : DependencyInjectionTest
    {
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
                    var repositoryBuilder = x.UseEfCore<EfCoreContext>(o => { });
                    repositoryBuilder.AsDefault<EfCoreContext>();
                    repositoryBuilder.AsIEfCoreRepository<EfCoreContext>();

                    x.UseDapper().AsIDapperRepository(typeof(User).Assembly);
                });

            services.BuildServiceProvider();

            // Assert
            AssertSpecifiedServiceTypeAndImplementationType<IEfCoreRepository<User, long>, EfCoreRepositoryBase<EfCoreContext, User, long>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IEfCoreRepository<Role, int>, EfCoreRepositoryBase<EfCoreContext, Role, int>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IEfCoreRepository<Role>, EfCoreRepositoryBase<EfCoreContext, Role>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IEfCoreRepository<User, long>, EfCoreRepositoryBase<EfCoreContext, User, long>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IEfCoreRepository<Role, int>, EfCoreRepositoryBase<EfCoreContext, Role, int>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IEfCoreRepository<Role>, EfCoreRepositoryBase<EfCoreContext, Role>>(services, ServiceLifetime.Transient);

            AssertSpecifiedServiceTypeAndImplementationType<IRepository<User, long>, EfCoreRepositoryBase<EfCoreContext, User, long>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<Role, int>, EfCoreRepositoryBase<EfCoreContext, Role, int>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<Role>, EfCoreRepositoryBase<EfCoreContext, Role>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<User, long>, EfCoreRepositoryBase<EfCoreContext, User, long>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<Role, int>, EfCoreRepositoryBase<EfCoreContext, Role, int>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<Role>, EfCoreRepositoryBase<EfCoreContext, Role>>(services, ServiceLifetime.Transient);

            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<User, long>, DapperRepositoryBase<User, long>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<Role, int>, DapperRepositoryBase<Role, int>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<Role>, DapperRepositoryBase<Role>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<User, long>, DapperRepositoryBase<User, long>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<Role, int>, DapperRepositoryBase<Role, int>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IDapperRepository<Role>, DapperRepositoryBase<Role>>(services, ServiceLifetime.Transient);
        }

        [Fact]
        public void TestAddDoubleRepositoryAsDefault()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton(CommonTest.GetHostingEnvironment());

            // Act
            services
                .AddEasyNet(x =>
                {
                    x.UseEfCore<EfCoreContext>(o => { }).AsDefault<EfCoreContext>();
                    x.UseDapper().AsDefault(typeof(User).Assembly);
                });

            services.BuildServiceProvider();

            // Assert
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<User, long>, EfCoreRepositoryBase<EfCoreContext, User, long>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<Role, int>, EfCoreRepositoryBase<EfCoreContext, Role, int>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<Role>, EfCoreRepositoryBase<EfCoreContext, Role>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<User, long>, EfCoreRepositoryBase<EfCoreContext, User, long>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<Role, int>, EfCoreRepositoryBase<EfCoreContext, Role, int>>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<IRepository<Role>, EfCoreRepositoryBase<EfCoreContext, Role>>(services, ServiceLifetime.Transient);
        }
    }
}
