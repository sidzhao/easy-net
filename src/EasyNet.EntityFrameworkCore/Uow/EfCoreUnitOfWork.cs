using System.Threading.Tasks;
using EasyNet.Data;
using EasyNet.EventBus;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.Runtime.Session;
using EasyNet.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace EasyNet.EntityFrameworkCore.Uow
{
    /// <summary>
    /// Implements Unit of work for Entity Framework.
    /// </summary>
    public class EfCoreUnitOfWork : UnitOfWorkBase
    {
        protected readonly IDbConnectorCreator DbConnectorCreator;

        public EfCoreUnitOfWork(
            IDbConnectorCreator dbConnectorCreator,
            IEasyNetSession session,
            IEasyNetEventMessageBuffer eventMessageBuffer,
            IOptions<UnitOfWorkDefaultOptions> defaultOptions) : base(session, eventMessageBuffer, defaultOptions)
        {
            DbConnectorCreator = dbConnectorCreator;
        }

        protected DbContext ActiveDbContext => DbConnector?.GetDbContext();

        protected IDbContextTransaction ActiveTransaction => DbConnector?.GetDbContextTransaction();

        public override void SaveChanges()
        {
            ActiveDbContext?.SaveChanges();
        }

        public override Task SaveChangesAsync()
        {
            if (ActiveDbContext != null)
            {
                return ActiveDbContext.SaveChangesAsync();
            }

            return Task.CompletedTask;
        }

        protected override void CompleteUow()
        {
            SaveChanges();
            CommitTransaction();
        }

        protected override async Task CompleteUowAsync()
        {
            await SaveChangesAsync();
            await CommitTransactionAsync();
        }

        protected virtual void CommitTransaction()
        {
            ActiveTransaction?.Commit();
        }

        protected virtual Task CommitTransactionAsync()
        {
#if Net461 || NetCore21
            ActiveTransaction?.Commit();

            return Task.CompletedTask;
#else
            if (ActiveTransaction != null)
            {
                return ActiveTransaction.CommitAsync();
            }

            return Task.CompletedTask;
#endif
        }

        protected override void DisposeUow()
        {
            // Do nothing
        }
    }
}
