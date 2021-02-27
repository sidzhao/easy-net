using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using EasyNet.CommonTests.Common.Entities;
using EasyNet.Data.Entities;
using EasyNet.Data.Repositories;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.Uow;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasyNet.Data.Tests.Base
{
    public abstract class SetTenantTest
    {
        protected IServiceProvider ServiceProvider;

        [Fact]
        public virtual void TestSetTenantId()
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
            Assert.Equal(2, usersNormal.Count);
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

                // Assert
                Assert.Equal(4, users.Count);
                Assert.Equal(2, roles.Count);

                if (UseEfCoreSaveChanges())
                {
                    var userAndRoles = (from u in userRepo.GetAll()
                                        join r in roleRepo.GetAll() on u.RoleId equals r.Id
                                        select u).ToList();
                    Assert.Empty(userAndRoles);
                }
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
             

                // Assert
                Assert.Equal(2, users.Count);
                Assert.Equal(2, roles.Count);
            
                if (UseEfCoreSaveChanges())
                {
                    var userAndRoles = (from u in userRepo.GetAll() join r in roleRepo.GetAll() on u.RoleId equals r.Id select u).ToList();
                    Assert.Single(userAndRoles);
                }
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
        public virtual async Task TestSetTenantIdAsync()
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
            Assert.Equal(2, usersNormal.Count);
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
              

                // Assert
                Assert.Equal(4, users.Count);
                Assert.Equal(2, roles.Count);
                
                if (UseEfCoreSaveChanges())
                {
                    var userAndRoles = await (from u in userRepo.GetAll() join r in roleRepo.GetAll() on u.RoleId equals r.Id select u).ToListAsync();
                    Assert.Empty(userAndRoles);
                }
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
               
                // Assert
                Assert.Equal(2, users.Count);
                Assert.Equal(2, roles.Count);
                
                if (UseEfCoreSaveChanges())
                {
                    var userAndRoles = await (from u in userRepo.GetAll() join r in roleRepo.GetAll() on u.RoleId equals r.Id select u).ToListAsync();
                    Assert.Single(userAndRoles);
                }
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

        protected virtual DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        protected virtual bool UseEfCoreSaveChanges()
        {
            return true;
        }

        protected IUnitOfWorkCompleteHandle BeginUow(UnitOfWorkOptions options = null)
        {
            return ServiceProvider.GetService<IUnitOfWorkManager>().Begin(ServiceProvider, options ?? new UnitOfWorkOptions());
        }

        protected virtual IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity<int>
        {
            return ServiceProvider.GetService<IRepository<TEntity>>();
        }

        protected virtual IRepository<TEntity, TPrimaryKey> GetRepository<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>
        {
            return ServiceProvider.GetService<IRepository<TEntity, TPrimaryKey>>();
        }
    }
}
