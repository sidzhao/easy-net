using System;
using System.Data.Common;
using System.Threading.Tasks;
using EasyNet.CommonTests;
using EasyNet.CommonTests.Core;
using EasyNet.CommonTests.Core.Entities;
using EasyNet.Data;
using EasyNet.DependencyInjection;
using EasyNet.EntityFrameworkCore.Tests.DbContext;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.Uow;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasyNet.EntityFrameworkCore.Tests
{
    public class EfCoreDataFilterTest
    {
        private readonly IServiceProvider _serviceProvider;

        public EfCoreDataFilterTest()
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
                .AddSession<TestSession>()
                .AddCurrentDbConnectorProvider<TestCurrentDbConnectorProvider>();

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void TestMustHaveTenantFilter()
        {
            // Arrange
            using var uow = BeginUow();
            var userRepo = GetRepository<User, long>();

            #region Enable MustHaveTenant

            // Act
            var count1 = userRepo.Count();

            // Assert
            Assert.Equal(2, count1);

            #endregion

            #region Disable MustHaveTenant

            using (((IActiveUnitOfWork)uow).DisableFilter(EasyNetDataFilters.MustHaveTenant))
            {
                // Act
                var count2 = userRepo.Count();

                // Assert
                Assert.Equal(6, count2);

                #region Enable MustHaveTenant

                using (((IActiveUnitOfWork)uow).EnableFilter(EasyNetDataFilters.MustHaveTenant))
                {
                    // Act
                    var count3 = userRepo.Count();

                    // Assert
                    Assert.Equal(2, count3);
                }

                #endregion
            }

            #endregion

            // Complete uow
            uow.Complete();
        }

        [Fact]
        public async Task TestMustHaveTenantFilterAsync()
        {
            // Arrange
            using var uow = BeginUow();
            var userRepo = GetRepository<User, long>();

            #region Enable MustHaveTenant

            // Act
            var count1 = await userRepo.CountAsync();

            // Assert
            Assert.Equal(2, count1);

            #endregion

            #region Disable MustHaveTenant

            using (((IActiveUnitOfWork)uow).DisableFilter(EasyNetDataFilters.MustHaveTenant))
            {
                // Act
                var count2 = await userRepo.CountAsync();

                // Assert
                Assert.Equal(6, count2);

                #region Enable MustHaveTenant

                using (((IActiveUnitOfWork)uow).EnableFilter(EasyNetDataFilters.MustHaveTenant))
                {
                    // Act
                    var count3 = await userRepo.CountAsync();

                    // Assert
                    Assert.Equal(2, count3);
                }

                #endregion
            }

            #endregion

            // Complete uow
            await uow.CompleteAsync();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestMustHaveTenantFilterThroughUowOptions(bool enableMustHaveTenantFilter)
        {
            // Arrange
            var uowOptions = new UnitOfWorkOptions();
            uowOptions.FilterOverrides.Add(new DataFilterConfiguration(EasyNetDataFilters.MustHaveTenant, enableMustHaveTenantFilter));

            using var uow = BeginUow(uowOptions);
            var userRepo = GetRepository<User, long>();

            // Act
            var count = userRepo.Count();

            // Assert
            Assert.Equal(enableMustHaveTenantFilter ? 2 : 6, count);

            // Complete uow
            uow.Complete();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestMustHaveTenantFilterThroughUowOptionsAsync(bool enableMustHaveTenantFilter)
        {
            // Arrange
            var uowOptions = new UnitOfWorkOptions();
            uowOptions.FilterOverrides.Add(new DataFilterConfiguration(EasyNetDataFilters.MustHaveTenant, enableMustHaveTenantFilter));

            using var uow = BeginUow(uowOptions);
            var userRepo = GetRepository<User, long>();

            // Act
            var count = await userRepo.CountAsync();

            // Assert
            Assert.Equal(enableMustHaveTenantFilter ? 2 : 6, count);

            // Complete uow
            await uow.CompleteAsync();
        }

        [Fact]
        public void TestMayHaveTenantFilter()
        {
            // Arrange
            using var uow = BeginUow();
            var roleRepo = GetRepository<Role>();

            #region Enable MayHaveTenant

            // Act
            var count1 = roleRepo.Count();

            // Assert
            Assert.Equal(2, count1);

            #endregion

            #region Disable MayHaveTenant

            using (((IActiveUnitOfWork)uow).DisableFilter(EasyNetDataFilters.MayHaveTenant))
            {
                // Act
                var count2 = roleRepo.Count();

                // Assert
                Assert.Equal(4, count2);

                #region Enable MayHaveTenant

                using (((IActiveUnitOfWork)uow).EnableFilter(EasyNetDataFilters.MayHaveTenant))
                {
                    // Act
                    var count3 = roleRepo.Count();

                    // Assert
                    Assert.Equal(2, count3);
                }

                #endregion
            }

            #endregion

            // Complete uow
            uow.Complete();
        }

        [Fact]
        public async Task TestMayHaveTenantFilterAsync()
        {
            // Arrange
            using var uow = BeginUow();
            var roleRepo = GetRepository<Role>();

            #region Enable MayHaveTenant

            // Act
            var count1 = await roleRepo.CountAsync();

            // Assert
            Assert.Equal(2, count1);

            #endregion

            #region Disable MayHaveTenant

            using (((IActiveUnitOfWork)uow).DisableFilter(EasyNetDataFilters.MayHaveTenant))
            {
                // Act
                var count2 = await roleRepo.CountAsync();

                // Assert
                Assert.Equal(4, count2);

                #region Enable MayHaveTenant

                using (((IActiveUnitOfWork)uow).EnableFilter(EasyNetDataFilters.MayHaveTenant))
                {
                    // Act
                    var count3 = await roleRepo.CountAsync();

                    // Assert
                    Assert.Equal(2, count3);
                }

                #endregion
            }

            #endregion

            // Complete uow
            await uow.CompleteAsync();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestMayHaveTenantFilterThroughUowOptions(bool enableMayHaveTenantFilter)
        {
            // Arrange
            var uowOptions = new UnitOfWorkOptions();
            uowOptions.FilterOverrides.Add(new DataFilterConfiguration(EasyNetDataFilters.MayHaveTenant, enableMayHaveTenantFilter));

            using var uow = BeginUow(uowOptions);
            var roleRepo = GetRepository<Role>();

            // Act
            var count = roleRepo.Count();

            // Assert
            Assert.Equal(enableMayHaveTenantFilter ? 2 : 4, count);

            // Complete uow
            uow.Complete();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestMayHaveTenantFilterThroughUowOptionsAsync(bool enableMayHaveTenantFilter)
        {
            // Arrange
            var uowOptions = new UnitOfWorkOptions();
            uowOptions.FilterOverrides.Add(new DataFilterConfiguration(EasyNetDataFilters.MayHaveTenant, enableMayHaveTenantFilter));

            using var uow = BeginUow(uowOptions);
            var roleRepo = GetRepository<Role>();

            // Act
            var count = await roleRepo.CountAsync();

            // Assert
            Assert.Equal(enableMayHaveTenantFilter ? 2 : 4, count);

            // Complete uow
            await uow.CompleteAsync();
        }

        [Fact]
        public void TestSoftDeleteFilter()
        {
            // Arrange
            using var uow = BeginUow();
            var deletionAuditedRepo = GetRepository<TestDeletionAudited>();

            #region Enable SoftDelete

            // Act
            var count1 = deletionAuditedRepo.Count();

            // Assert
            Assert.Equal(5, count1);

            #endregion

            #region Disable SoftDelete

            using (((IActiveUnitOfWork)uow).DisableFilter(EasyNetDataFilters.SoftDelete))
            {
                // Act
                var count2 = deletionAuditedRepo.Count();

                // Assert
                Assert.Equal(6, count2);
            }

            #endregion

            // Complete uow
            uow.Complete();
        }

        [Fact]
        public async Task TestSoftDeleteFilterAsync()
        {
            // Arrange
            using var uow = BeginUow();
            var deletionAuditedRepo = GetRepository<TestDeletionAudited>();

            #region Enable SoftDelete

            // Act
            var count1 = await deletionAuditedRepo.CountAsync();

            // Assert
            Assert.Equal(5, count1);

            #endregion

            #region Disable SoftDelete

            using (((IActiveUnitOfWork)uow).DisableFilter(EasyNetDataFilters.SoftDelete))
            {
                // Act
                var count2 = await deletionAuditedRepo.CountAsync();

                // Assert
                Assert.Equal(6, count2);
            }

            #endregion

            // Complete uow
            await uow.CompleteAsync();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestSoftDeleteFilterThroughUowOptions(bool enableSoftDelete)
        {
            // Arrange
            var uowOptions = new UnitOfWorkOptions();
            uowOptions.FilterOverrides.Add(new DataFilterConfiguration(EasyNetDataFilters.SoftDelete, enableSoftDelete));

            using var uow = BeginUow(uowOptions);
            var deletionAuditedRepo = GetRepository<TestDeletionAudited>();

            // Act
            var count = deletionAuditedRepo.Count();

            // Assert
            Assert.Equal(enableSoftDelete ? 5 : 6, count);

            // Complete uow
            uow.Complete();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestSoftDeleteFilterThroughUowOptionsAsync(bool enableSoftDelete)
        {
            // Arrange
            var uowOptions = new UnitOfWorkOptions();
            uowOptions.FilterOverrides.Add(new DataFilterConfiguration(EasyNetDataFilters.SoftDelete, enableSoftDelete));

            using var uow = BeginUow(uowOptions);
            var deletionAuditedRepo = GetRepository<TestDeletionAudited>();

            // Act
            var count = await deletionAuditedRepo.CountAsync();

            // Assert
            Assert.Equal(enableSoftDelete ? 5 : 6, count);

            // Complete uow
            await uow.CompleteAsync();
        }

        private DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public IUnitOfWorkCompleteHandle BeginUow(UnitOfWorkOptions options = null)
        {
            return _serviceProvider.GetService<IUnitOfWorkManager>().Begin(_serviceProvider, options ?? new UnitOfWorkOptions());
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity<int>
        {
            return _serviceProvider.GetService<IRepository<TEntity>>();
        }

        public IRepository<TEntity, TPrimaryKey> GetRepository<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>
        {
            return _serviceProvider.GetService<IRepository<TEntity, TPrimaryKey>>();
        }
    }
}
