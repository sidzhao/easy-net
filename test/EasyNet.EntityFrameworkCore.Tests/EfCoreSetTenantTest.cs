using System;
using System.Data.Common;
using EasyNet.DependencyInjection;
using EasyNet.Domain.Entities;
using EasyNet.Domain.Repositories;
using EasyNet.Domain.Uow;
using EasyNet.EntityFrameworkCore.Extensions;
using EasyNet.EntityFrameworkCore.Tests.DbContext;
using EasyNet.EntityFrameworkCore.Tests.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using EasyNet.Extensions.DependencyInjection;

namespace EasyNet.EntityFrameworkCore.Tests
{
    public class EfCoreSetTenantTest
    {
        private readonly IServiceProvider _serviceProvider;

        public EfCoreSetTenantTest()
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

            InitData();
        }

        [Fact]
        public void TestSetTenantId()
        {
            // Arrange
            using var uow = BeginUow();
            var userRepo = GetRepository<User, long>();
            var roleRepo = GetRepository<Role>();

            #region Normal

            // Act
            var usersNormal = userRepo.GetAllList();
            var rolesNormal = roleRepo.GetAllList();

            // Assert
            Assert.Single(usersNormal);
            Assert.Equal(2, rolesNormal.Count);

            #endregion

            #region TenantId = 2

            using (((IActiveUnitOfWork)uow).SetTenantId("2"))
            {
                // Act
                userRepo.InsertAndGetId(new User
                {
                    Name = "Test1",
                    RoleId = 2
                });
                roleRepo.InsertAndGetId(new Role
                {
                    Name = "TestRole1",
                });

                var users = userRepo.GetAllList();
                var roles = roleRepo.GetAllList();
                var userAndRoles = (from u in userRepo.GetAll() join r in roleRepo.GetAll() on u.RoleId equals r.Id select u).ToList();

                // Assert
                Assert.Equal(4, users.Count);
                Assert.Equal(2, roles.Count);
                Assert.Empty(userAndRoles);
            }

            #endregion

            #region TenantId = 3

            using (((IActiveUnitOfWork)uow).SetTenantId("3"))
            {
                // Act
                userRepo.InsertAndGetId(new User
                {
                    Name = "Test1",
                    RoleId = 2
                });
                roleRepo.InsertAndGetId(new Role
                {
                    Name = "TestRole1",
                });

                var users = userRepo.GetAllList();
                var roles = roleRepo.GetAllList();
                var userAndRoles = (from u in userRepo.GetAll() join r in roleRepo.GetAll() on u.RoleId equals r.Id select u).ToList();

                // Assert
                Assert.Equal(2, users.Count);
                Assert.Equal(2, roles.Count);
                Assert.Single(userAndRoles);
            }

            #endregion

            #region TenantId = null

            using (((IActiveUnitOfWork)uow).SetTenantId(string.Empty))
            {
                // Act
                roleRepo.InsertAndGetId(new Role
                {
                    Name = "NullRole"
                });
                var users = userRepo.GetAllList();
                var roles = roleRepo.GetAllList();

                // Assert
                Assert.Throws<EasyNetException>(() => userRepo.InsertAndGetId(new User
                {
                    Name = "Test1",
                    RoleId = 2
                }));
                Assert.Empty(users);
                Assert.Single(roles);
            }

            #endregion

            // Complete uow
            uow.Complete();
        }

        [Fact]
        public async Task TestSetTenantIdAsync()
        {
            // Arrange
            using var uow = BeginUow();
            var userRepo = GetRepository<User, long>();
            var roleRepo = GetRepository<Role>();

            #region Normal

            // Act
            var usersNormal = await userRepo.GetAllListAsync();
            var rolesNormal = await roleRepo.GetAllListAsync();

            // Assert
            Assert.Single(usersNormal);
            Assert.Equal(2, rolesNormal.Count);

            #endregion

            #region TenantId = 2

            using (((IActiveUnitOfWork)uow).SetTenantId("2"))
            {
                // Act
                await userRepo.InsertAndGetIdAsync(new User
                {
                    Name = "Test1",
                    RoleId = 2
                });
                await roleRepo.InsertAndGetIdAsync(new Role
                {
                    Name = "TestRole1",
                });

                var users = await userRepo.GetAllListAsync();
                var roles = await roleRepo.GetAllListAsync();
                var userAndRoles = await (from u in userRepo.GetAll() join r in roleRepo.GetAll() on u.RoleId equals r.Id select u).ToListAsync();

                // Assert
                Assert.Equal(4, users.Count);
                Assert.Equal(2, roles.Count);
                Assert.Empty(userAndRoles);
            }

            #endregion

            #region TenantId = 3

            using (((IActiveUnitOfWork)uow).SetTenantId("3"))
            {
                // Act
                await userRepo.InsertAndGetIdAsync(new User
                {
                    Name = "Test1",
                    RoleId = 2
                });
                await roleRepo.InsertAndGetIdAsync(new Role
                {
                    Name = "TestRole1",
                });

                var users = await userRepo.GetAllListAsync();
                var roles = await roleRepo.GetAllListAsync();
                var userAndRoles = await (from u in userRepo.GetAll() join r in roleRepo.GetAll() on u.RoleId equals r.Id select u).ToListAsync();

                // Assert
                Assert.Equal(2, users.Count);
                Assert.Equal(2, roles.Count);
                Assert.Single(userAndRoles);
            }

            #endregion

            #region TenantId = null

            using (((IActiveUnitOfWork)uow).SetTenantId(string.Empty))
            {
                // Act
                await roleRepo.InsertAndGetIdAsync(new Role
                {
                    Name = "NullRole"
                });
                var users = await userRepo.GetAllListAsync();
                var roles = await roleRepo.GetAllListAsync();

                // Assert
                await Assert.ThrowsAsync<EasyNetException>(async () => await userRepo.InsertAndGetIdAsync(new User
                {
                    Name = "Test1",
                    RoleId = 2
                }));
                Assert.Empty(users);
                Assert.Single(roles);
            }

            #endregion

            // Complete uow
            await uow.CompleteAsync();
        }

        private DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        private void InitData()
        {
            var context = _serviceProvider.GetService<EfCoreContext>();
            context.Database.EnsureCreated();

            // Insert default roles.
            context.Roles.Add(new Role { TenantId = 1, Name = "Admin" });
            context.SaveChanges();
            context.Roles.Add(new Role { TenantId = 1, Name = "Admin1" });
            context.SaveChanges();
            context.Roles.Add(new Role { TenantId = 2, Name = "User" });
            context.SaveChanges();
            context.Roles.Add(new Role { TenantId = 3, Name = "Client" });
            context.SaveChanges();

            // Insert default users.
            context.Users.Add(new User { TenantId = 1, Name = "User1", Status = Status.Active, RoleId = 1 });
            context.SaveChanges();
            context.Users.Add(new User { TenantId = 2, Name = "User2", Status = Status.Active, RoleId = 2 });
            context.SaveChanges();
            context.Users.Add(new User { TenantId = 2, Name = "User3", Status = Status.Inactive, RoleId = 2 });
            context.SaveChanges();
            context.Users.Add(new User { TenantId = 2, Name = "User4", Status = Status.Active, RoleId = 2 });
            context.SaveChanges();
            context.Users.Add(new User { TenantId = 3, Name = "User5", Status = Status.Active, RoleId = 4 });
            context.SaveChanges();

            // Insert default test modification audited records.
            context.TestModificationAudited.Add(new TestModificationAudited { Name = "Update1" });
            context.SaveChanges();
            context.TestModificationAudited.Add(new TestModificationAudited { Name = "Update2" });
            context.SaveChanges();
            context.TestModificationAudited.Add(new TestModificationAudited { Name = "Update3" });
            context.SaveChanges();

            // Insert default test deletion audited records.
            context.TestDeletionAudited.Add(new TestDeletionAudited { IsActive = true });
            context.SaveChanges();
            context.TestDeletionAudited.Add(new TestDeletionAudited { IsActive = false });
            context.SaveChanges();
            context.TestDeletionAudited.Add(new TestDeletionAudited { IsActive = true });
            context.SaveChanges();
            context.TestDeletionAudited.Add(new TestDeletionAudited { IsActive = true });
            context.SaveChanges();
            context.TestDeletionAudited.Add(new TestDeletionAudited { IsActive = false });
            context.SaveChanges();
            context.TestDeletionAudited.Add(new TestDeletionAudited { IsActive = false, IsDeleted = true });
            context.SaveChanges();

            // Clear all change trackers
            foreach (var entry in context.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
        }

        public IUnitOfWorkCompleteHandle BeginUow(UnitOfWorkOptions options = null)
        {
            return _serviceProvider.GetService<IUnitOfWorkManager>().Begin(options ?? new UnitOfWorkOptions());
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
