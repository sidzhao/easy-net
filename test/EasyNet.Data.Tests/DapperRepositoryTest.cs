using System;
using System.Linq;
using System.Threading.Tasks;
using EasyNet.Dapper.Data;
using EasyNet.Data.Tests.Base;
using EasyNet.Data.Tests.Core.Data;
using EasyNet.Data.Tests.Core.Data.Entities;
using EasyNet.DependencyInjection;
using EasyNet.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasyNet.Data.Tests
{
    public class DapperRepositoryTest : RepositoryTest
    {
        public DapperRepositoryTest()
        {
            var services = new ServiceCollection();

            services
                .AddEasyNet(x =>
                {
                    x.Assemblies = new[] { this.GetType().Assembly };
                    x.UseSqlLite("Filename=:memory:");
                    x.UseDapper();
                })
                .AddSession<TestSession>()
                .AddCurrentDbConnectorProvider<TestCurrentDbConnectorProvider>();

            ServiceProvider = services.BuildServiceProvider();
        }

        #region GetAll

        [Fact]
        public void TestGetAllWithSql()
        {
            // Arrange
            var userRepo = GetDapperRepository<User, long>();

            // Act
            var users = userRepo.GetAllList("SELECT * FROM Users WHERE Status=@Status", new
            {
                Status = Status.Active
            }).ToList();

            // Assert
            Assert.Equal(4, users.Count);
            Assert.Equal("User3", users[1].Name);
        }

        [Fact]
        public async Task TestGetAllWithSqlAsync()
        {
            // Arrange
            var userRepo = GetDapperRepository<User, long>();

            // Act
            var users = (await userRepo.GetAllListAsync("SELECT * FROM Users WHERE Status=@Status", new
            {
                Status = Status.Active
            })).ToList();

            // Assert
            Assert.Equal(4, users.Count);
            Assert.Equal("User3", users[1].Name);
        }

        #endregion

        #region Single

        [Fact]
        public void TestSingleWithSql()
        {
            // Arrange
            var userRepo = GetDapperRepository<User, long>();

            // Act
            var user2 = userRepo.Single("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User2"
            });

            var user4 = userRepo.Single("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User4"
            });

            // Assert
            Assert.NotNull(user2);
            Assert.NotNull(user4);
        }

        [Fact]
        public async Task TestSingleWithSqlAsync()
        {
            // Arrange
            var userRepo = GetDapperRepository<User, long>();

            // Act
            var user2 = await userRepo.SingleAsync("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User2"
            });

            var user4 = await userRepo.SingleAsync("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User4"
            });

            // Assert
            Assert.NotNull(user2);
            Assert.NotNull(user4);
        }

        #endregion

        #region SingleOrDefault

        [Fact]
        public void TestSingleOrDefaultWithSql()
        {
            // Arrange
            var userRepo = GetDapperRepository<User, long>();

            // Act
            var user2 = userRepo.SingleOrDefault("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User2"
            });
            var user4 = userRepo.SingleOrDefault("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User4"
            });
            var user0 = userRepo.SingleOrDefault("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User0"
            });

            // Assert
            Assert.NotNull(user4);
            Assert.NotNull(user2);
            Assert.Null(user0);
        }

        [Fact]
        public async Task TestSingleOrDefaultWithSqlAsync()
        {
            // Arrange
            var userRepo = GetDapperRepository<User, long>();

            // Act
            var user2 = await userRepo.SingleOrDefaultAsync("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User2"
            });
            var user4 = await userRepo.SingleOrDefaultAsync("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User4"
            });
            var user0 = await userRepo.SingleOrDefaultAsync("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User0"
            });

            // Assert
            Assert.NotNull(user4);
            Assert.NotNull(user2);
            Assert.Null(user0);
        }

        #endregion

        #region First

        [Fact]
        public void TestFirstWithSql()
        {
            // Arrange
            var userRepo = GetDapperRepository<User, long>();

            // Act
            var user = userRepo.First("SELECT * FROM Users");
            var inactiveUser = userRepo.First("SELECT * FROM Users WHERE Status=@Status", new
            {
                Status = Status.Inactive
            });

            // Assert
            Assert.NotNull(user);
            Assert.NotNull(inactiveUser);
            Assert.Equal("User1", user.Name);
            Assert.Equal("User2", inactiveUser.Name);
            Assert.Throws<InvalidOperationException>(() => userRepo.First("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User0"
            }));
        }

        [Fact]
        public async Task TestFirstWithSqlAsync()
        {
            // Arrange
            var userRepo = GetDapperRepository<User, long>();

            // Act
            var user = await userRepo.FirstAsync("SELECT * FROM Users");
            var inactiveUser = await userRepo.FirstAsync("SELECT * FROM Users WHERE Status=@Status", new
            {
                Status = Status.Inactive
            });

            // Assert
            Assert.NotNull(user);
            Assert.NotNull(inactiveUser);
            Assert.Equal("User1", user.Name);
            Assert.Equal("User2", inactiveUser.Name);
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await userRepo.FirstAsync("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User0"
            }));
        }

        #endregion

        #region FirstOrDefault

        [Fact]
        public void TestFirstOrDefaultWithSql()
        {
            // Arrange
            var userRepo = GetDapperRepository<User, long>();

            // Act
            var user = userRepo.FirstOrDefault("SELECT * FROM Users");
            var inactiveUser = userRepo.FirstOrDefault("SELECT * FROM Users WHERE Status=@Status", new
            {
                Status = Status.Inactive
            });
            var nullUser = userRepo.FirstOrDefault("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User0"
            });

            // Assert
            Assert.NotNull(user);
            Assert.NotNull(inactiveUser);
            Assert.Null(nullUser);
            Assert.Equal("User1", user.Name);
            Assert.Equal("User2", inactiveUser.Name);
        }

        [Fact]
        public async Task TestFirstOrDefaultWithSqlAsync()
        {
            // Arrange
            var userRepo = GetDapperRepository<User, long>();

            // Act
            var user = await userRepo.FirstOrDefaultAsync("SELECT * FROM Users");
            var inactiveUser = await userRepo.FirstOrDefaultAsync("SELECT * FROM Users WHERE Status=@Status", new
            {
                Status = Status.Inactive
            });
            var nullUser = await userRepo.FirstOrDefaultAsync("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User0"
            });

            // Assert
            Assert.NotNull(user);
            Assert.NotNull(inactiveUser);
            Assert.Null(nullUser);
            Assert.Equal("User1", user.Name);
            Assert.Equal("User2", inactiveUser.Name);
        }

        #endregion

        public IDapperRepository<TEntity> GetDapperRepository<TEntity>() where TEntity : class, IEntity<int>
        {
            return ServiceProvider.GetService<IDapperRepository<TEntity>>();
        }

        public IDapperRepository<TEntity, TPrimaryKey> GetDapperRepository<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>
        {
            return ServiceProvider.GetService<IDapperRepository<TEntity, TPrimaryKey>>();
        }
    }
}
