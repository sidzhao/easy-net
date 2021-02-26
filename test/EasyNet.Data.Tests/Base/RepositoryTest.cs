using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using EasyNet.CommonTests.Common.Entities;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.Uow;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasyNet.Data.Tests.Base
{
    public abstract class RepositoryTest
    {
        protected IServiceProvider ServiceProvider;

        #region Get

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestGet(bool useUow)
        {
            void Do()
            {
                // Arrange
                var roleRepo = GetRepository<Role>();

                // Act
                var role2 = roleRepo.Get(2);

                // Assert
                Assert.Equal("Admin1", role2.Name);

                if (useUow)
                {
                    try
                    {
                        roleRepo.Get(3);
                    }
                    catch (Exception ex)
                    {
                        Assert.True(ex is EasyNetNotFoundEntityException<Role, int> || ex is InvalidOperationException);
                    }
                }
                else
                {
                    Assert.NotNull(roleRepo.Get(3));
                }
            }

            if (useUow)
            {
                using var uow = BeginUow();

                Do();

                uow.Complete();
            }
            else
            {
                Do();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestGetAsync(bool useUow)
        {
            async Task DoAsync()
            {
                // Arrange
                var roleRepo = GetRepository<Role>();

                // Act
                var role2 = await roleRepo.GetAsync(2);

                // Assert
                Assert.Equal("Admin1", role2.Name);

                if (useUow)
                {
                    try
                    {
                        await roleRepo.GetAsync(3);
                    }
                    catch (Exception ex)
                    {
                        Assert.True(ex is EasyNetNotFoundEntityException<Role, int> || ex is InvalidOperationException);
                    }
                }
                else
                {
                    Assert.NotNull(roleRepo.GetAsync(3));
                }
            }

            if (useUow)
            {
                using var uow = BeginUow();

                await DoAsync();

                await uow.CompleteAsync();
            }
            else
            {
                await DoAsync();
            }
        }

        #endregion

        #region GetAllList

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestGetAllList(bool useUow)
        {
            void Do()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                // Act
                var users = userRepo.GetAllList();

                // Assert
                Assert.Equal(useUow ? 2 : 6, users.Count);
                Assert.Equal("User2", users[1].Name);
            }

            if (useUow)
            {
                using var uow = BeginUow();

                Do();

                uow.Complete();
            }
            else
            {
                Do();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestGetAllListByPredicate(bool useUow)
        {
            void Do()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                // Act
                var users = userRepo.GetAllList(p => p.Status == Status.Active);
                var users1 = userRepo.GetAllList(p => p.Status == Status.Active || p.RoleId == 1);

                // Assert
                Assert.Equal(useUow ? 1 : 4, users.Count);
                Assert.Equal(useUow ? 2 : 5, users1.Count);
                if (useUow)
                {
                    Assert.Equal("User1", users[0].Name);
                }
                else
                {
                    Assert.Equal("User3", users[1].Name);
                }
            }

            if (useUow)
            {
                using var uow = BeginUow();

                Do();

                uow.Complete();
            }
            else
            {
                Do();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestGetAllListAsync(bool useUow)
        {
            async Task DoAsync()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                // Act
                var users = await userRepo.GetAllListAsync();

                // Assert
                Assert.Equal(useUow ? 2 : 6, users.Count);
                Assert.Equal("User2", users[1].Name);
            }

            if (useUow)
            {
                using var uow = BeginUow();

                await DoAsync();

                await uow.CompleteAsync();
            }
            else
            {
                await DoAsync();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestGetAllListByPredicateAsync(bool useUow)
        {

            async Task DoAsync()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                // Act
                var users = await userRepo.GetAllListAsync(p => p.Status == Status.Active);
                var users1 = await userRepo.GetAllListAsync(p => p.Status == Status.Active || p.RoleId == 1);

                // Assert
                Assert.Equal(useUow ? 1 : 4, users.Count);
                Assert.Equal(useUow ? 2 : 5, users1.Count);
                if (useUow)
                {
                    Assert.Equal("User1", users[0].Name);
                }
                else
                {
                    Assert.Equal("User3", users[1].Name);
                }
            }

            if (useUow)
            {
                using var uow = BeginUow();

                await DoAsync();

                await uow.CompleteAsync();
            }
            else
            {
                await DoAsync();
            }
        }

        #endregion

        #region Signle

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestSingle(bool useUow)
        {
            void Do()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                // Act
                var user = userRepo.Single(p => p.Name == "User2");

                // Assert
                Assert.NotNull(user);
                if (useUow)
                {
                    Assert.Throws<InvalidOperationException>(
                        () => userRepo.Single(p => p.Name == "User4"));
                }
                else
                {
                    Assert.NotNull(userRepo.Single(p => p.Name == "User4"));
                }
            }

            if (useUow)
            {
                using var uow = BeginUow();

                Do();

                uow.Complete();
            }
            else
            {
                Do();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestSingleAsync(bool useUow)
        {
            async Task DoAsync()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                // Act
                var user = await userRepo.SingleAsync(p => p.Name == "User2");

                // Assert
                Assert.NotNull(user);

                if (useUow)
                {
                    await Assert.ThrowsAsync<InvalidOperationException>(
                        async () => await userRepo.SingleAsync(p => p.Name == "User4"));
                }
                else
                {
                    Assert.NotNull(await userRepo.SingleAsync(p => p.Name == "User4"));
                }
            }

            if (useUow)
            {
                using var uow = BeginUow();

                await DoAsync();

                await uow.CompleteAsync();
            }
            else
            {
                await DoAsync();
            }
        }

        #endregion

        #region SignleOrDefault

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestSingleOrDefault(bool useUow)
        {
            void Do()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                // Act
                var user2 = userRepo.SingleOrDefault(p => p.Name == "User2");
                var user4 = userRepo.SingleOrDefault(p => p.Name == "User4");
                var user0 = userRepo.SingleOrDefault(p => p.Name == "User0");

                // Assert
                if (useUow)
                {
                    Assert.Null(user4);
                }
                else
                {
                    Assert.NotNull(user4);
                }
                Assert.NotNull(user2);
                Assert.Null(user0);
            }

            if (useUow)
            {
                using var uow = BeginUow();

                Do();

                uow.Complete();
            }
            else
            {
                Do();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestSingleOrDefaultAsync(bool useUow)
        {
            async Task DoAsync()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                // Act
                var user2 = await userRepo.SingleOrDefaultAsync(p => p.Name == "User2");
                var user4 = await userRepo.SingleOrDefaultAsync(p => p.Name == "User4");
                var user0 = await userRepo.SingleOrDefaultAsync(p => p.Name == "User0");

                // Assert
                if (useUow)
                {
                    Assert.Null(user4);
                }
                else
                {
                    Assert.NotNull(user4);
                }
                Assert.NotNull(user2);
                Assert.Null(user0);
            }

            if (useUow)
            {
                using var uow = BeginUow();

                await DoAsync();

                await uow.CompleteAsync();
            }
            else
            {
                await DoAsync();
            }
        }

        #endregion

        #region First

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestFirst(bool useUow)
        {
            void Do()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                // Act
                var user = userRepo.First();
                var inactiveUser = userRepo.First(p => p.Status == Status.Inactive);

                // Assert
                Assert.NotNull(user);
                Assert.NotNull(inactiveUser);
                Assert.Equal("User1", user.Name);
                Assert.Equal("User2", inactiveUser.Name);
                Assert.Throws<InvalidOperationException>(() => userRepo.First(p => p.Name == "User0"));
            }

            if (useUow)
            {
                using var uow = BeginUow();

                Do();

                uow.Complete();
            }
            else
            {
                Do();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestFirstAsync(bool useUow)
        {
            async Task DoAsync()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                // Act
                var user = await userRepo.FirstAsync();
                var inactiveUser = await userRepo.FirstAsync(p => p.Status == Status.Inactive);

                // Assert
                Assert.NotNull(user);
                Assert.NotNull(inactiveUser);
                Assert.Equal("User1", user.Name);
                Assert.Equal("User2", inactiveUser.Name);
                await Assert.ThrowsAsync<InvalidOperationException>(
                    async () => await userRepo.FirstAsync(p => p.Name == "User0"));
            }

            if (useUow)
            {
                using var uow = BeginUow();

                await DoAsync();

                await uow.CompleteAsync();
            }
            else
            {
                await DoAsync();
            }
        }

        #endregion

        #region FirstOrDefault

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestFirstOrDefault(bool useUow)
        {
            void Do()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                // Act
                var user = userRepo.FirstOrDefault();
                var inactiveUser = userRepo.FirstOrDefault(p => p.Status == Status.Inactive);
                var nullUser = userRepo.FirstOrDefault(p => p.Name == "User0");

                // Assert
                Assert.NotNull(user);
                Assert.NotNull(inactiveUser);
                Assert.Null(nullUser);
                Assert.Equal("User1", user.Name);
                Assert.Equal("User2", inactiveUser.Name);
            }

            if (useUow)
            {
                using var uow = BeginUow();

                Do();

                uow.Complete();
            }
            else
            {
                Do();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestFirstOrDefaultAsync(bool useUow)
        {
            async Task DoAsync()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                // Act
                var user = await userRepo.FirstOrDefaultAsync();
                var inactiveUser = await userRepo.FirstOrDefaultAsync(p => p.Status == Status.Inactive);
                var nullUser = await userRepo.FirstOrDefaultAsync(p => p.Name == "User0");

                // Assert
                Assert.NotNull(user);
                Assert.NotNull(inactiveUser);
                Assert.Null(nullUser);
                Assert.Equal("User1", user.Name);
                Assert.Equal("User2", inactiveUser.Name);
            }

            if (useUow)
            {
                using var uow = BeginUow();

                await DoAsync();

                await uow.CompleteAsync();
            }
            else
            {
                await DoAsync();
            }
        }

        #endregion

        #region Count

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestCount(bool useUow)
        {
            void Do()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();
                var roleRepo = GetRepository<Role>();

                // Act
                var count = userRepo.Count();
                var activeCount = userRepo.Count(p => p.Status == Status.Active);
                var zeroCount = userRepo.Count(p => p.Name == "Zero");
                var roleCount = roleRepo.Count();

                // Assert
                Assert.Equal(useUow ? 2 : 6, count);
                Assert.Equal(useUow ? 1 : 4, activeCount);
                Assert.Equal(0, zeroCount);
                Assert.Equal(useUow ? 2 : 4, roleCount);
            }

            if (useUow)
            {
                using var uow = BeginUow();

                Do();

                uow.Complete();
            }
            else
            {
                Do();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestCountAsync(bool useUow)
        {
            async Task DoAsync()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();
                var roleRepo = GetRepository<Role>();

                // Act
                var count = await userRepo.CountAsync();
                var activeCount = await userRepo.CountAsync(p => p.Status == Status.Active);
                var zeroCount = await userRepo.CountAsync(p => p.Name == "Zero");
                var roleCount = await roleRepo.CountAsync();

                // Assert
                Assert.Equal(useUow ? 2 : 6, count);
                Assert.Equal(useUow ? 1 : 4, activeCount);
                Assert.Equal(0, zeroCount);
                Assert.Equal(useUow ? 2 : 4, roleCount);
            }

            if (useUow)
            {
                using var uow = BeginUow();

                await DoAsync();

                await uow.CompleteAsync();
            }
            else
            {
                await DoAsync();
            }
        }

        #endregion

        #region LongCount

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestLongCount(bool useUow)
        {
            void Do()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                // Act
                var count = userRepo.LongCount();
                var activeCount = userRepo.LongCount(p => p.Status == Status.Active);
                var zeroCount = userRepo.Count(p => p.Name == "Zero");

                // Assert
                Assert.Equal(useUow ? 2 : 6, count);
                Assert.Equal(useUow ? 1 : 4, activeCount);
                Assert.Equal(0, zeroCount);
            }

            if (useUow)
            {
                using var uow = BeginUow();

                Do();

                uow.Complete();
            }
            else
            {
                Do();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestLongCountAsync(bool useUow)
        {
            async Task DoAsync()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                // Act
                var count = await userRepo.LongCountAsync();
                var activeCount = await userRepo.LongCountAsync(p => p.Status == Status.Active);
                var zeroCount = await userRepo.CountAsync(p => p.Name == "Zero");

                // Assert
                Assert.Equal(useUow ? 2 : 6, count);
                Assert.Equal(useUow ? 1 : 4, activeCount);
                Assert.Equal(0, zeroCount);
            }

            if (useUow)
            {
                using var uow = BeginUow();

                await DoAsync();

                await uow.CompleteAsync();
            }
            else
            {
                await DoAsync();
            }
        }

        #endregion

        #region Any

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestAny(bool useUow)
        {
            void Do()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                // Act
                var activeAny = userRepo.Any(p => p.Status == Status.Active);
                var zeroAny = userRepo.Any(p => p.Name == "Zero");

                // Assert
                Assert.True(activeAny);
                Assert.False(zeroAny);
            }

            if (useUow)
            {
                using var uow = BeginUow();

                Do();

                uow.Complete();
            }
            else
            {
                Do();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestAnyAsync(bool useUow)
        {
            async Task DoAsync()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                // Act
                var activeAny = await userRepo.AnyAsync(p => p.Status == Status.Active);
                var zeroAny = await userRepo.AnyAsync(p => p.Name == "Zero");

                // Assert
                Assert.True(activeAny);
                Assert.False(zeroAny);
            }

            if (useUow)
            {
                using var uow = BeginUow();

                await DoAsync();

                await uow.CompleteAsync();
            }
            else
            {
                await DoAsync();
            }
        }

        #endregion

        #region Insert

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestInsert(bool useUow)
        {
            var isEfCoreTest = GetType().Name == "EfCoreRepositoryTest";

            void Do()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();
                var roleRepo = GetRepository<Role>();
                var creationAuditedRepo = GetRepository<TestCreationAudited>();

                #region Insert but not SaveChanges

                // Act
                var user = new User
                {
                    Name = "TempUser1",
                    RoleId = 1
                };
                userRepo.Insert(user);

                var role = new Role
                {
                    Name = "TempRole1",
                };
                roleRepo.Insert(role);

                var creationAudited1 = new TestCreationAudited
                {
                    Name = "TempCreation1"
                };
                creationAuditedRepo.Insert(creationAudited1);

                // Assert
                if (isEfCoreTest)
                {
                    Assert.Equal(0, user.Id);
                    Assert.Equal(useUow ? 2 : 6, userRepo.Count());

                    Assert.Equal(0, role.Id);
                    Assert.Equal(useUow ? 2 : 4, roleRepo.Count());

                    Assert.Equal(0, creationAudited1.Id);
                    Assert.Equal(3, creationAuditedRepo.Count());
                }

                #endregion

                #region SaveChanges

                // Act
                if (isEfCoreTest)
                    GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChanges();

                // Assert
                Assert.Equal(7, user.Id);
                Assert.NotNull(userRepo.SingleOrDefault(p => p.Id == 7));
                Assert.Equal(useUow ? 3 : 7, userRepo.Count());

                Assert.Equal(5, role.Id);
                Assert.NotNull(roleRepo.SingleOrDefault(p => p.Id == 5));
                Assert.Equal(useUow ? 3 : 5, roleRepo.Count());

                Assert.Equal(4, creationAudited1.Id);
                Assert.Equal(1, creationAuditedRepo.SingleOrDefault(p => p.Id == 4)?.CreatorUserId);
                Assert.Equal(4, creationAuditedRepo.Count());

                #endregion
            }

            if (useUow)
            {
                using var uow = BeginUow();

                Do();

                uow.Complete();
            }
            else
            {
                Do();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestInsertAsync(bool useUow)
        {
            async Task DoAsync()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();
                var roleRepo = GetRepository<Role>();
                var creationAuditedRepo = GetRepository<TestCreationAudited>();

                #region Insert but not SaveChanges

                // Act
                var user = new User
                {
                    Name = "TempUser1",
                    RoleId = 1
                };
                await userRepo.InsertAsync(user);

                var role = new Role
                {
                    Name = "TempRole1",
                };
                await roleRepo.InsertAsync(role);

                var creationAudited1 = new TestCreationAudited
                {
                    Name = "TempCreation1"
                };
                await creationAuditedRepo.InsertAsync(creationAudited1);

                // Assert
                Assert.Equal(0, user.Id);
                Assert.Equal(useUow ? 2 : 6, await userRepo.GetAll().AsNoTracking().CountAsync());

                Assert.Equal(0, role.Id);
                Assert.Equal(useUow ? 2 : 4, await roleRepo.GetAll().AsNoTracking().CountAsync());

                Assert.Equal(0, creationAudited1.Id);
                Assert.Equal(3, await creationAuditedRepo.GetAll().AsNoTracking().CountAsync());

                #endregion

                #region SaveChanges

                // Act
                await GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChangesAsync();

                // Assert
                Assert.Equal(7, user.Id);
                Assert.Equal(useUow ? 3 : 7, await userRepo.GetAll().AsNoTracking().CountAsync());
                Assert.NotNull(await userRepo.GetAll().AsNoTracking().SingleOrDefaultAsync(p => p.Id == 7));

                Assert.Equal(5, role.Id);
                Assert.NotNull(await roleRepo.GetAll().AsNoTracking().SingleOrDefaultAsync(p => p.Id == 5));
                Assert.Equal(useUow ? 3 : 5, await roleRepo.GetAll().AsNoTracking().CountAsync());

                Assert.Equal(4, creationAudited1.Id);
                Assert.Equal(1, (await creationAuditedRepo.GetAll().AsNoTracking().SingleOrDefaultAsync(p => p.Id == 4))?.CreatorUserId);
                Assert.Equal(4, await creationAuditedRepo.GetAll().AsNoTracking().CountAsync());

                #endregion
            }

            if (useUow)
            {
                using var uow = BeginUow();

                await DoAsync();

                await uow.CompleteAsync();
            }
            else
            {
                await DoAsync();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestInsertAndGetId(bool useUow)
        {
            void Do()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();
                var roleRepo = GetRepository<Role>();
                var creationAuditedRepo = GetRepository<TestCreationAudited>();

                // Act
                var role = new Role
                {
                    Name = "TempRole1"
                };
                roleRepo.InsertAndGetId(role);

                var user = new User
                {
                    Name = "TempUser1",
                    RoleId = role.Id
                };
                userRepo.InsertAndGetId(user);

                var creationAudited1 = new TestCreationAudited
                {
                    Name = "TempCreation"
                };
                creationAuditedRepo.InsertAndGetId(creationAudited1);

                // Assert
                Assert.Equal(5, role.Id);
                Assert.NotNull(roleRepo.GetAll().AsNoTracking().SingleOrDefault(p => p.Id == 5));
                Assert.Equal(useUow ? 3 : 5, roleRepo.GetAll().AsNoTracking().Count());

                Assert.Equal(7, user.Id);
                Assert.NotNull(userRepo.GetAll().AsNoTracking().SingleOrDefault(p => p.Id == 7));
                Assert.Equal(useUow ? 3 : 7, userRepo.GetAll().AsNoTracking().Count());

                Assert.Equal(4, creationAudited1.Id);
                Assert.Equal(1, creationAuditedRepo.GetAll().AsNoTracking().SingleOrDefault(p => p.Id == 4)?.CreatorUserId);
                Assert.Equal(4, creationAuditedRepo.GetAll().AsNoTracking().Count());
            }

            if (useUow)
            {
                using var uow = BeginUow();

                Do();

                uow.Complete();
            }
            else
            {
                Do();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestInsertAndGetIdAsync(bool useUow)
        {
            async Task DoAsync()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();
                var roleRepo = GetRepository<Role>();
                var creationAuditedRepo = GetRepository<TestCreationAudited>();

                // Act
                var role = new Role
                {
                    Name = "TempRole1"
                };
                await roleRepo.InsertAndGetIdAsync(role);

                var user = new User
                {
                    Name = "TempUser1",
                    RoleId = 1
                };
                await userRepo.InsertAndGetIdAsync(user);

                var creationAudited1 = new TestCreationAudited
                {
                    Name = "TempCreation"
                };
                await creationAuditedRepo.InsertAndGetIdAsync(creationAudited1);

                // Assert
                Assert.Equal(5, role.Id);
                Assert.NotNull(await roleRepo.GetAll().AsNoTracking().SingleOrDefaultAsync(p => p.Id == 5));
                Assert.Equal(useUow ? 3 : 5, await roleRepo.GetAll().AsNoTracking().CountAsync());

                Assert.Equal(7, user.Id);
                Assert.Equal(useUow ? 3 : 7, await userRepo.GetAll().AsNoTracking().CountAsync());
                Assert.NotNull(await userRepo.GetAll().AsNoTracking().SingleOrDefaultAsync(p => p.Id == 7));

                Assert.Equal(4, creationAudited1.Id);
                Assert.Equal(1, (await creationAuditedRepo.GetAll().AsNoTracking().SingleOrDefaultAsync(p => p.Id == 4))?.CreatorUserId);
                Assert.Equal(4, await creationAuditedRepo.GetAll().AsNoTracking().CountAsync());
            }

            if (useUow)
            {
                using var uow = BeginUow();

                await DoAsync();

                await uow.CompleteAsync();
            }
            else
            {
                await DoAsync();
            }
        }

        #endregion

        #region Update

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestUpdate(bool useUow)
        {
            var isEfCoreTest = GetType().Name == "EfCoreRepositoryTest";

            void Do(IActiveUnitOfWork uow)
            {
                // Arrange
                var userRepo = GetRepository<User, long>();
                var roleRepo = GetRepository<Role>();
                var modificationAuditedRepo = GetRepository<TestModificationAudited, long>();

                #region Get then update

                // Act
                var user1 = userRepo.Get(1);
                user1.Name = "TestUser1";
                userRepo.Update(user1);

                var modificationAudited1 = modificationAuditedRepo.Get(1);
                modificationAudited1.Name = "TestUpdate1";
                modificationAuditedRepo.Update(modificationAudited1);

                if (isEfCoreTest)
                    GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChanges();

                // Assert
                Assert.Equal("TestUser1", userRepo.Single(p => p.Id == user1.Id).Name);

                Assert.Equal(1, modificationAuditedRepo.Single(p => p.Id == modificationAudited1.Id).LastModifierUserId);

                #endregion

                #region Attach

                // Act
                var user2 = new User
                {
                    Id = 2,
                    TenantId = 1,
                    Name = "TestUser2",
                    RoleId = 1
                };
                userRepo.Update(user2);

                var role1 = new Role
                {
                    Id = 1,
                    TenantId = null,
                    Name = "Admin1"
                };
                roleRepo.Update(role1);

                var modificationAudited2 = new TestModificationAudited
                {
                    Id = 2,
                    Name = "TestUpdate2"
                };
                modificationAudited2.Name = "TestUpdate2";
                modificationAuditedRepo.Update(modificationAudited2);

                if (isEfCoreTest)
                    GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChanges();

                // Assert
                Assert.Equal("TestUser2", userRepo.Single(p => p.Id == user2.Id).Name);
                Assert.Equal(useUow ? 1 : 4, roleRepo.Count());
                Assert.Equal(1, modificationAuditedRepo.Single(p => p.Id == modificationAudited2.Id).LastModifierUserId);
                if (useUow)
                {
                    using (uow.DisableFilter(EasyNetDataFilters.MayHaveTenant))
                    {
                        Assert.Equal(4, roleRepo.Count());
                    }
                }

                #endregion

                #region Update with action

                if (!useUow)
                {
                    // Act
                    userRepo.Update(3, user => { user.Name = "TestUser3"; });

                    modificationAuditedRepo.Update(3, user => { user.Name = "TestUpdate3"; });

                    if (isEfCoreTest)
                        GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChanges();

                    // Assert
                    Assert.Equal("TestUser3", userRepo.Single(p => p.Id == 3).Name);
                    Assert.Equal(1,
                        modificationAuditedRepo.Single(p => p.Id == 3).LastModifierUserId);

                }

                #endregion
            }

            if (useUow)
            {
                using var uow = BeginUow();

                Do((IActiveUnitOfWork)uow);

                uow.Complete();
            }
            else
            {
                Do(null);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestUpdateAsync(bool useUow)
        {
            var isEfCoreTest = GetType().Name == "EfCoreRepositoryTest";

            async Task DoAsync(IActiveUnitOfWork uow)
            {
                // Arrange
                var userRepo = GetRepository<User, long>();
                var roleRepo = GetRepository<Role>();
                var modificationAuditedRepo = GetRepository<TestModificationAudited, long>();

                #region Get then update

                // Act
                var user1 = await userRepo.GetAsync(1);
                user1.Name = "TestUser1";
                await userRepo.UpdateAsync(user1);

                var modificationAudited1 = await modificationAuditedRepo.GetAsync(1);
                modificationAudited1.Name = "TestUpdate1";
                await modificationAuditedRepo.UpdateAsync(modificationAudited1);

                if (isEfCoreTest)
                    await GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChangesAsync();

                // Assert
                Assert.Equal("TestUser1", (await userRepo.SingleAsync(p => p.Id == user1.Id)).Name);

                Assert.Equal(1, (await modificationAuditedRepo.SingleAsync(p => p.Id == modificationAudited1.Id)).LastModifierUserId);

                #endregion

                #region Attach

                // Act
                var user2 = new User
                {
                    Id = 2,
                    TenantId = 1,
                    Name = "TestUser2",
                    RoleId = 1
                };
                await userRepo.UpdateAsync(user2);

                var role1 = new Role
                {
                    Id = 1,
                    TenantId = null,
                    Name = "Admin1"
                };
                await roleRepo.UpdateAsync(role1);

                var modificationAudited2 = new TestModificationAudited
                {
                    Id = 2,
                    Name = "TestUpdate2"
                };
                modificationAudited2.Name = "TestUpdate2";
                await modificationAuditedRepo.UpdateAsync(modificationAudited2);

                if (isEfCoreTest)
                    await GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChangesAsync();

                // Assert
                Assert.Equal("TestUser2", (await userRepo.SingleAsync(p => p.Id == user2.Id)).Name);
                Assert.Equal(useUow ? 1 : 4, await roleRepo.CountAsync());
                Assert.Equal(1, (await modificationAuditedRepo.SingleAsync(p => p.Id == modificationAudited2.Id)).LastModifierUserId);
                if (useUow)
                {
                    using (uow.DisableFilter(EasyNetDataFilters.MayHaveTenant))
                    {
                        Assert.Equal(4, await roleRepo.CountAsync());
                    }
                }

                #endregion

                #region Update with action

                if (!useUow)
                {
                    // Act
                    await userRepo.UpdateAsync(3, user =>
                    {
                        user.Name = "TestUser3";
                        return Task.CompletedTask;
                    });

                    await modificationAuditedRepo.UpdateAsync(3, audited =>
                    {
                        audited.Name = "TestUpdate3";

                        return Task.CompletedTask;
                    });

                    if (isEfCoreTest)
                        await GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChangesAsync();

                    // Assert
                    Assert.Equal("TestUser3",
                        (await userRepo.SingleAsync(p => p.Id == 3)).Name);

                    Assert.Equal(1,
                        (await modificationAuditedRepo.SingleAsync(p => p.Id == 3)).LastModifierUserId);
                }


                #endregion
            }

            if (useUow)
            {
                using var uow = BeginUow();

                await DoAsync((IActiveUnitOfWork)uow);

                await uow.CompleteAsync();
            }
            else
            {
                await DoAsync(null);
            }
        }

        #endregion

        #region InsertOrUpdate

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestInsertOrUpdate(bool useUow)
        {
            void Do()
            {
                // Arrange
                var modificationAuditedRepo = GetRepository<TestModificationAudited, long>();

                // Act
                var modificationAudited1 = new TestModificationAudited
                {
                    Id = 1,
                    Name = "TestUpdate1"
                };
                modificationAuditedRepo.InsertOrUpdate(modificationAudited1);

                var modificationAudited4 = new TestModificationAudited
                {
                    Name = "TestUpdate4"
                };
                modificationAuditedRepo.InsertOrUpdate(modificationAudited4);

                GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChanges();

                // Assert
                Assert.Equal(4, modificationAuditedRepo.Count());
                Assert.Equal("TestUpdate1", modificationAuditedRepo.GetAll().AsNoTracking().Single(p => p.Id == 1).Name);
                Assert.Equal(1, modificationAuditedRepo.GetAll().AsNoTracking().Single(p => p.Id == 1).LastModifierUserId);
                Assert.Equal("TestUpdate4", modificationAuditedRepo.GetAll().AsNoTracking().Single(p => p.Id == 4).Name);
                Assert.Equal(1, modificationAuditedRepo.GetAll().AsNoTracking().Single(p => p.Id == 4).CreatorUserId);
            }

            if (useUow)
            {
                using var uow = BeginUow();

                Do();

                uow.Complete();
            }
            else
            {
                Do();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestInsertOrUpdateAsync(bool useUow)
        {
            async Task DoAsync()
            {
                // Arrange
                var modificationAuditedRepo = GetRepository<TestModificationAudited, long>();

                // Act
                var modificationAudited1 = new TestModificationAudited
                {
                    Id = 1,
                    Name = "TestUpdate1"
                };
                await modificationAuditedRepo.InsertOrUpdateAsync(modificationAudited1);

                var modificationAudited4 = new TestModificationAudited
                {
                    Name = "TestUpdate4"
                };
                await modificationAuditedRepo.InsertOrUpdateAsync(modificationAudited4);

                await GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChangesAsync();

                // Assert
                Assert.Equal(4, await modificationAuditedRepo.CountAsync());
                Assert.Equal("TestUpdate1", (await modificationAuditedRepo.GetAll().AsNoTracking().SingleAsync(p => p.Id == 1)).Name);
                Assert.Equal(1, (await modificationAuditedRepo.GetAll().AsNoTracking().SingleAsync(p => p.Id == 1)).LastModifierUserId);
                Assert.Equal("TestUpdate4", (await modificationAuditedRepo.GetAll().AsNoTracking().SingleAsync(p => p.Id == 4)).Name);
                Assert.Equal(1, (await modificationAuditedRepo.GetAll().AsNoTracking().SingleAsync(p => p.Id == 4)).CreatorUserId);
            }

            if (useUow)
            {
                using var uow = BeginUow();

                await DoAsync();

                await uow.CompleteAsync();
            }
            else
            {
                await DoAsync();
            }
        }

        #endregion

        #region Hard Delete

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestHardDelete(bool useUow)
        {
            var isEfCoreTest = GetType().Name == "EfCoreRepositoryTest";

            void Do()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                #region Delete by id

                // Act
                userRepo.Delete(1);

                if (isEfCoreTest)
                    GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChanges();

                // Assert
                Assert.Null(userRepo.SingleOrDefault(p => p.Id == 1));
                Assert.Equal(useUow ? 1 : 5, userRepo.Count());

                #endregion

                #region Delete by entity

                // Act
                userRepo.Delete(userRepo.Get(2));

                if (isEfCoreTest)
                    GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChanges();

                // Assert
                Assert.Null(userRepo.SingleOrDefault(p => p.Id == 2));
                Assert.Equal(useUow ? 0 : 4, userRepo.Count());

                #endregion

                #region Delete by predicate

                // Act
                userRepo.Delete(p => p.RoleId == 2);

                if (isEfCoreTest)
                    GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChanges();

                // Assert
                Assert.Equal(useUow ? 0 : 1, userRepo.Count());

                #endregion
            }

            if (useUow)
            {
                using var uow = BeginUow();

                Do();

                uow.Complete();
            }
            else
            {
                Do();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestHardDeleteAsync(bool useUow)
        {
            var isEfCoreTest = GetType().Name == "EfCoreRepositoryTest";

            async Task DoAsync()
            {
                // Arrange
                var userRepo = GetRepository<User, long>();

                #region Delete by id

                // Act
                await userRepo.DeleteAsync(1);

                if (isEfCoreTest)
                    await GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChangesAsync();

                // Assert
                Assert.Null(await userRepo.SingleOrDefaultAsync(p => p.Id == 1));
                Assert.Equal(useUow ? 1 : 5, await userRepo.CountAsync());

                #endregion

                #region Delete by entity

                // Act
                await userRepo.DeleteAsync(await userRepo.GetAsync(2));

                if (isEfCoreTest)
                    await GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChangesAsync();

                // Assert
                Assert.Null(await userRepo.SingleOrDefaultAsync(p => p.Id == 2));
                Assert.Equal(useUow ? 0 : 4, await userRepo.CountAsync());

                #endregion

                #region Delete by predicate

                // Act
                await userRepo.DeleteAsync(p => p.RoleId == 2);

                if (isEfCoreTest)
                    await GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChangesAsync();

                // Assert
                Assert.Equal(useUow ? 0 : 1, await userRepo.CountAsync());

                #endregion
            }

            if (useUow)
            {
                using var uow = BeginUow();

                await DoAsync();

                await uow.CompleteAsync();
            }
            else
            {
                await DoAsync();
            }
        }

        #endregion

        #region SoftDelet

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestSoftDelete(bool useUow)
        {
            var isEfCoreTest = GetType().Name == "EfCoreRepositoryTest";

            void Do()
            {
                // Arrange
                var deletionAuditedRepo = GetRepository<TestDeletionAudited>();

                #region Delete by id

                // Act
                deletionAuditedRepo.Delete(1);

                if (isEfCoreTest)
                    GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChanges();

                // Assert
                Assert.Equal(1, deletionAuditedRepo.Single(p => p.Id == 1).DeleterUserId);
                Assert.Equal(2, deletionAuditedRepo.Count(p => p.IsDeleted));
                Assert.Equal(6, deletionAuditedRepo.Count());

                #endregion

                #region Delete by entity

                // Act
                deletionAuditedRepo.Delete(deletionAuditedRepo.Get(2));

                if (isEfCoreTest)
                    GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChanges();

                // Assert
                Assert.Equal(1, deletionAuditedRepo.Single(p => p.Id == 2).DeleterUserId);
                Assert.Equal(3, deletionAuditedRepo.Count(p => p.IsDeleted));
                Assert.Equal(6, deletionAuditedRepo.Count());

                #endregion

                #region Delete by predicate

                // Act
                deletionAuditedRepo.Delete(p => p.IsActive == false);

                if (isEfCoreTest)
                    GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChanges();

                // Assert
                Assert.Equal(3,
                    deletionAuditedRepo.Count(p => p.IsActive == false && p.IsDeleted));
                Assert.Equal(6, deletionAuditedRepo.Count());

                #endregion
            }

            if (useUow)
            {
                using var uow = BeginUow();

                using (((IActiveUnitOfWork)uow).DisableFilter(EasyNetDataFilters.SoftDelete))
                {
                    Do();
                }

                uow.Complete();
            }
            else
            {
                Do();
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TestSoftDeleteAsync(bool useUow)
        {
            var isEfCoreTest = GetType().Name == "EfCoreRepositoryTest";

            async Task DoAsync()
            {
                // Arrange
                var deletionAuditedRepo = GetRepository<TestDeletionAudited>();

                #region Delete by id

                // Act
                await deletionAuditedRepo.DeleteAsync(1);

                if (isEfCoreTest)
                    await GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChangesAsync();

                // Assert
                Assert.Equal(1,
                    (await deletionAuditedRepo.SingleAsync(p => p.Id == 1)).DeleterUserId);
                Assert.Equal(2, await deletionAuditedRepo.CountAsync(p => p.IsDeleted));
                Assert.Equal(6, await deletionAuditedRepo.CountAsync());

                #endregion

                #region Delete by entity

                // Act
                await deletionAuditedRepo.DeleteAsync(await deletionAuditedRepo.GetAsync(2));

                if (isEfCoreTest)
                    await GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChangesAsync();

                // Assert
                Assert.Equal(1,
                    (await deletionAuditedRepo.SingleAsync(p => p.Id == 2)).DeleterUserId);
                Assert.Equal(3, await deletionAuditedRepo.CountAsync(p => p.IsDeleted));
                Assert.Equal(6, await deletionAuditedRepo.CountAsync());

                #endregion

                #region Delete by predicate

                // Act
                await deletionAuditedRepo.DeleteAsync(p => p.IsActive == false);

                if (isEfCoreTest)
                    await GetCurrentDbConnectorProvider().Current.GetDbContext().SaveChangesAsync();

                // Assert
                Assert.Equal(3,
                    await deletionAuditedRepo.CountAsync(p => p.IsActive == false && p.IsDeleted));
                Assert.Equal(6, await deletionAuditedRepo.CountAsync());

                #endregion
            }

            if (useUow)
            {
                using var uow = BeginUow();

                using (((IActiveUnitOfWork)uow).DisableFilter(EasyNetDataFilters.SoftDelete))
                {
                    await DoAsync();
                }

                await uow.CompleteAsync();
            }
            else
            {
                await DoAsync();
            }
        }

        #endregion

        protected DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public IUnitOfWorkCompleteHandle BeginUow()
        {
            return ServiceProvider.GetService<IUnitOfWorkManager>().Begin(ServiceProvider);
        }

        public virtual IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity<int>
        {
            return ServiceProvider.GetService<IRepository<TEntity>>();
        }

        public virtual IRepository<TEntity, TPrimaryKey> GetRepository<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>
        {
            return ServiceProvider.GetService<IRepository<TEntity, TPrimaryKey>>();
        }

        public ICurrentDbConnectorProvider GetCurrentDbConnectorProvider()
        {
            return ServiceProvider.GetRequiredService<ICurrentDbConnectorProvider>();
        }
    }
}
