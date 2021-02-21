using System.Threading.Tasks;
using EasyNet.Domain.Uow;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.Ioc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace EasyNet.EntityFrameworkCore.Domain.Uow
{
    /// <summary>
    /// Implements Unit of work for Entity Framework.
    /// </summary>
    public class EfCoreUnitOfWork : UnitOfWorkBase
    {
        protected DbContext ActiveDbContext { get; private set; }

        protected IDbContextTransaction ActiveTransaction { get; private set; }

        public EfCoreUnitOfWork(IIocResolver iocResolver, IOptions<UnitOfWorkDefaultOptions> defaultOptions) : base(iocResolver, defaultOptions)
        {
        }

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

        public virtual DbContext GetOrCreateDbContext()
        {
            if (ActiveDbContext == null)
            {
                ActiveDbContext = IocResolver.GetService<IDbContextCreator>().CreateDbContext();

                if (Options.IsTransactional == true)
                {
                    ActiveTransaction = BeginTransaction(ActiveDbContext);
                }
            }

            return ActiveDbContext;
        }

        protected virtual IDbContextTransaction BeginTransaction(DbContext dbContext)
        {
            return ActiveDbContext.Database.BeginTransaction((Options.IsolationLevel ?? System.Transactions.IsolationLevel.ReadUncommitted).ToSystemDataIsolationLevel());
        }

        protected override void DisposeUow()
        {
            ActiveTransaction?.Dispose();
            ActiveDbContext?.Dispose();
        }
    }
}
