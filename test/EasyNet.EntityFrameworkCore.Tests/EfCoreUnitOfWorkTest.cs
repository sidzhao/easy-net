using System;
using System.Data.Common;
using System.Threading.Tasks;
using EasyNet.CommonTests.Common;
using EasyNet.Data;
using EasyNet.DependencyInjection;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.Uow;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasyNet.EntityFrameworkCore.Tests
{
    public class EfCoreUnitOfWorkTest
    {
        private readonly IServiceProvider _serviceProvider;

        public EfCoreUnitOfWorkTest()
        {
            var services = new ServiceCollection();

            services
                .AddEasyNet(x =>
                {
                    x.UseEfCore<EfCoreContext>(options =>
                    {
                        options.UseSqlite(CreateInMemoryDatabase());
                    });
                })
                .AddSession<TestSession>();

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void TestBegin()
        {
            #region Without DbContext and Transaction

            // Arrange
            var uow1 = GetEfCoreUnitOfWork();

            // Act
            uow1.Begin(new UnitOfWorkOptions());
            uow1.Complete();

            #endregion

            #region With DbContext but Transaction

            // Arrange
            var uow2 = GetEfCoreUnitOfWork();

            // Act
            uow2.Begin(new UnitOfWorkOptions { IsTransactional = false });
            GetCurrentDbConnectorProvider().GetOrCreate();
            uow2.Complete();

            // Assert
            Assert.NotNull(uow2.GetPrivateProperty<Microsoft.EntityFrameworkCore.DbContext>("ActiveDbContext"));
            Assert.Null(uow2.GetPrivateProperty<IDbContextTransaction>("ActiveTransaction"));

            #endregion

            #region With DbContext and Transaction

            // Arrange
            var uow3 = GetEfCoreUnitOfWork();

            // Act
            uow3.Begin(new UnitOfWorkOptions { IsTransactional = true });
            GetCurrentDbConnectorProvider().GetOrCreate();
            uow3.Complete();

            // Assert
            Assert.NotNull(uow3.GetPrivateProperty<Microsoft.EntityFrameworkCore.DbContext>("ActiveDbContext"));
            Assert.NotNull(uow3.GetPrivateProperty<IDbContextTransaction>("ActiveTransaction"));

            #endregion
        }

        [Fact]
        public async Task TestBeginAsync()
        {
            #region Without DbContext and Transaction

            // Arrange
            var uow1 = GetEfCoreUnitOfWork();

            // Act
            uow1.Begin(new UnitOfWorkOptions());
            await uow1.CompleteAsync();

            #endregion

            #region With DbContext but Transaction

            // Arrange
            var uow2 = GetEfCoreUnitOfWork();

            // Act
            uow2.Begin(new UnitOfWorkOptions { IsTransactional = false });
            GetCurrentDbConnectorProvider().GetOrCreate();
            await uow2.CompleteAsync();

            // Assert
            Assert.NotNull(uow2.GetPrivateProperty<Microsoft.EntityFrameworkCore.DbContext>("ActiveDbContext"));
            Assert.Null(uow2.GetPrivateProperty<IDbContextTransaction>("ActiveTransaction"));

            #endregion

            #region With DbContext and Transaction

            // Arrange
            var uow3 = GetEfCoreUnitOfWork();

            // Act
            uow3.Begin(new UnitOfWorkOptions { IsTransactional = true });
            GetCurrentDbConnectorProvider().GetOrCreate();
            await uow3.CompleteAsync();

            // Assert
            Assert.NotNull(uow3.GetPrivateProperty<Microsoft.EntityFrameworkCore.DbContext>("ActiveDbContext"));
            Assert.NotNull(uow3.GetPrivateProperty<IDbContextTransaction>("ActiveTransaction"));

            #endregion
        }

        private IUnitOfWork GetEfCoreUnitOfWork()
        {
            return _serviceProvider.GetRequiredService<IUnitOfWork>();
        }

        private ICurrentDbConnectorProvider GetCurrentDbConnectorProvider()
        {
            return _serviceProvider.GetRequiredService<ICurrentDbConnectorProvider>();
        }

        private DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }
    }
}
