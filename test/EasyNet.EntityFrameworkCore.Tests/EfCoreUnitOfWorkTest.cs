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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestBegin(bool isTransactional)
        {
            // Arrange
            var uowManager = _serviceProvider.GetService<IUnitOfWorkManager>();
            var uow = uowManager.Begin(_serviceProvider, new UnitOfWorkOptions { IsTransactional = isTransactional });

            // Act
            GetCurrentDbConnectorProvider().GetOrCreate();
            uow.Complete();

            // Assert
            Assert.NotNull(uow.GetPrivateProperty<DbContext>("ActiveDbContext"));
            Assert.Equal(!isTransactional, uow.GetPrivateProperty<IDbContextTransaction>("ActiveTransaction") == null);

            // Act
            uow.Dispose();

            // Assert
            Assert.True(uow.GetPrivateProperty<DbContext>("ActiveDbContext").GetPrivateField<bool>("_disposed"));
            if (isTransactional)
            {
                Assert.True(uow.GetPrivateProperty<IDbContextTransaction>("ActiveTransaction").GetPrivateField<bool>("_disposed"));
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestBeginAsync(bool isTransactional)
        {
            // Arrange
            var uowManager = _serviceProvider.GetService<IUnitOfWorkManager>();
            var uow = uowManager.Begin(_serviceProvider, new UnitOfWorkOptions { IsTransactional = isTransactional });

            // Act
            GetCurrentDbConnectorProvider().GetOrCreate();
            await uow.CompleteAsync();

            // Assert
            Assert.NotNull(uow.GetPrivateProperty<DbContext>("ActiveDbContext"));
            Assert.Equal(!isTransactional, uow.GetPrivateProperty<IDbContextTransaction>("ActiveTransaction") == null);

            // Act
            uow.Dispose();

            // Assert
            Assert.True(uow.GetPrivateProperty<DbContext>("ActiveDbContext").GetPrivateField<bool>("_disposed"));
            if (isTransactional)
            {
                Assert.True(uow.GetPrivateProperty<IDbContextTransaction>("ActiveTransaction").GetPrivateField<bool>("_disposed"));
            }
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
