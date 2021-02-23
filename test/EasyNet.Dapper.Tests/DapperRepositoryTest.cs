using EasyNet.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using EasyNet.CommonTests.Core;
using EasyNet.Dapper.Data;
using EasyNet.Data;
using EasyNet.Uow;
using Xunit;

namespace EasyNet.Dapper.Tests
{
    public class DapperRepositoryTest
    {
        private readonly IServiceProvider _serviceProvider;

        public DapperRepositoryTest()
        {
            var services = new ServiceCollection();

            services
                .AddEasyNet(x =>
                {
                    x.Assemblies = new[] { this.GetType().Assembly };
                    x.UseSqlLite("Filename=:memory:");
                    x.UseDapper();
                });

            _serviceProvider = services.BuildServiceProvider();

            InitData();
        }

        [Fact]
        public void TestGetAll()
        {

        }

        private void InitData()
        {
            using var dbConnectorCreator = _serviceProvider.GetService<IDbConnectorCreator>().Create();
            var connection = dbConnectorCreator.Connection;

            DatabaseHelper.InitData(connection);
        }
        public IUnitOfWorkCompleteHandle BeginUow()
        {
            return _serviceProvider.GetService<IUnitOfWorkManager>().Begin(_serviceProvider);
        }

        public IDapperRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity<int>
        {
            return _serviceProvider.GetService<IDapperRepository<TEntity>>();
        }

        public IDapperRepository<TEntity, TPrimaryKey> GetRepository<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>
        {
            return _serviceProvider.GetService<IDapperRepository<TEntity, TPrimaryKey>>();
        }

        public ICurrentDbConnectorProvider GetCurrentDbConnectorProvider()
        {
            return _serviceProvider.GetRequiredService<ICurrentDbConnectorProvider>();
        }
    }
}
