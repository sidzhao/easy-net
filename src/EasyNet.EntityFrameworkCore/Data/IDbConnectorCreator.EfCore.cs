using System;
using System.Data;
using EasyNet.Data;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyNet.EntityFrameworkCore.Data
{
    public class EfCoreDbConnectorCreator<TDbContext> : IDbConnectorCreator where TDbContext : DbContext
    {
        protected readonly IServiceProvider ServiceProvider;

        public EfCoreDbConnectorCreator(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IDbConnector Create(UnitOfWorkOptions options = null)
        {
            var dbConnector = new EfCoreDbConnector
            {
                DbContext = ServiceProvider.GetRequiredService<TDbContext>()
            };

            if (options?.IsTransactional != null && options.IsTransactional.Value)
            {
                dbConnector.DbContextTransaction = dbConnector.DbContext.Database.BeginTransaction(
                    (options.IsolationLevel ?? System.Transactions.IsolationLevel.ReadUncommitted)
                    .ToSystemDataIsolationLevel());
            }

            return dbConnector;
        }

        public IDbConnector Create(bool beginTransaction = false, IsolationLevel? isolationLevel = null)
        {
            var dbConnector = new EfCoreDbConnector
            {
                DbContext = ServiceProvider.GetRequiredService<TDbContext>()
            };

            if (beginTransaction)
            {
                dbConnector.DbContextTransaction = isolationLevel != null ?
                    dbConnector.DbContext.Database.BeginTransaction(isolationLevel.Value) :
                    dbConnector.DbContext.Database.BeginTransaction();
            }

            return dbConnector;
        }
    }
}
