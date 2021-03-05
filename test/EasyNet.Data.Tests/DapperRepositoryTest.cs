using System;
using System.Linq;
using System.Threading.Tasks;
using EasyNet.CommonTests.Common;
using EasyNet.CommonTests.Common.Entities;
using EasyNet.Data.Entities;
using EasyNet.Data.Repositories;
using EasyNet.Data.Tests.Base;
using EasyNet.DependencyInjection;
using EasyNet.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
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
                    x.UseDapper().AsDefault(typeof(User).Assembly);
                })
                .AddSession<TestSession>()
                .AddCurrentDbConnectorProvider<TestCurrentDbConnectorProvider>();

            services.TryAddSingleton<ILoggerProvider, DebugLoggerProvider>();
            services.AddTransient(typeof(ILogger), sp => sp.GetService<ILoggerProvider>().CreateLogger(typeof(IRepository).Name));

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
            var user2 = userRepo.GetSingle("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User2"
            });

            var user4 = userRepo.GetSingle("SELECT * FROM Users WHERE Name=@Name", new
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
            var user2 = await userRepo.GetSingleAsync("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User2"
            });

            var user4 = await userRepo.GetSingleAsync("SELECT * FROM Users WHERE Name=@Name", new
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
            var user2 = userRepo.GetSingleOrDefault("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User2"
            });
            var user4 = userRepo.GetSingleOrDefault("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User4"
            });
            var user0 = userRepo.GetSingleOrDefault("SELECT * FROM Users WHERE Name=@Name", new
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
            var user2 = await userRepo.GetSingleOrDefaultAsync("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User2"
            });
            var user4 = await userRepo.GetSingleOrDefaultAsync("SELECT * FROM Users WHERE Name=@Name", new
            {
                Name = "User4"
            });
            var user0 = await userRepo.GetSingleOrDefaultAsync("SELECT * FROM Users WHERE Name=@Name", new
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
            var user = userRepo.GetFirst("SELECT * FROM Users");
            var inactiveUser = userRepo.GetFirst("SELECT * FROM Users WHERE Status=@Status", new
            {
                Status = Status.Inactive
            });

            // Assert
            Assert.NotNull(user);
            Assert.NotNull(inactiveUser);
            Assert.Equal("User1", user.Name);
            Assert.Equal("User2", inactiveUser.Name);
            Assert.Throws<InvalidOperationException>(() => userRepo.GetFirst("SELECT * FROM Users WHERE Name=@Name", new
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
            var user = await userRepo.GetFirstAsync("SELECT * FROM Users");
            var inactiveUser = await userRepo.GetFirstAsync("SELECT * FROM Users WHERE Status=@Status", new
            {
                Status = Status.Inactive
            });

            // Assert
            Assert.NotNull(user);
            Assert.NotNull(inactiveUser);
            Assert.Equal("User1", user.Name);
            Assert.Equal("User2", inactiveUser.Name);
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await userRepo.GetFirstAsync("SELECT * FROM Users WHERE Name=@Name", new
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
            var user = userRepo.GetFirstOrDefault("SELECT * FROM Users");
            var inactiveUser = userRepo.GetFirstOrDefault("SELECT * FROM Users WHERE Status=@Status", new
            {
                Status = Status.Inactive
            });
            var nullUser = userRepo.GetFirstOrDefault("SELECT * FROM Users WHERE Name=@Name", new
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
            var user = await userRepo.GetFirstOrDefaultAsync("SELECT * FROM Users");
            var inactiveUser = await userRepo.GetFirstOrDefaultAsync("SELECT * FROM Users WHERE Status=@Status", new
            {
                Status = Status.Inactive
            });
            var nullUser = await userRepo.GetFirstOrDefaultAsync("SELECT * FROM Users WHERE Name=@Name", new
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

        protected override bool UseEfCoreSaveChanges()
        {
            return false;
        }

        public virtual IRepository<TEntity, TPrimaryKey> GetDapperRepository<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>
        {
            return GetRepository<TEntity, TPrimaryKey>();
        }
    }
}
