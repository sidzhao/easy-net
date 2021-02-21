using System;
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
        protected readonly ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider;

        public EfCoreDbConnectorCreator(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            CurrentUnitOfWorkProvider = serviceProvider.GetRequiredService<ICurrentUnitOfWorkProvider>();
        }

        public IDbConnector Create()
        {
            var dbConnector = new EfCoreDbConnector
            {
                DbContext = ServiceProvider.GetRequiredService<TDbContext>()
            };

            if (CurrentUnitOfWorkProvider.Current != null)
            {
                dbConnector.DbContextTransaction = dbConnector.DbContext.Database.BeginTransaction(
                    (CurrentUnitOfWorkProvider.Current.Options.IsolationLevel ?? System.Transactions.IsolationLevel.ReadUncommitted)
                    .ToSystemDataIsolationLevel());
            }

            return dbConnector;
        }
    }
}
