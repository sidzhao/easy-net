using System.Data;
using System.Threading.Tasks;
using EasyNet.Runtime.Session;
using Microsoft.Extensions.Options;

namespace EasyNet.Uow
{
    /// <summary>
    /// Null implementation of unit of work.
    /// It's used if no component registered for <see cref="IUnitOfWork"/>.
    /// This ensures working EasyNet without a database.
    /// </summary>
    public sealed class DefaultUnitOfWork : UnitOfWorkBase
    {
        public DefaultUnitOfWork(IOptions<UnitOfWorkDefaultOptions> defaultOptions) : base(NullEasyNetSession.Instance, defaultOptions)
        {
        }

        private IDbTransaction ActiveTransaction => DbConnector?.Transaction;

        public override void SaveChanges()
        {

        }

        public override Task SaveChangesAsync()
        {
            return Task.FromResult(0);
        }

        protected override void CompleteUow()
        {
            ActiveTransaction?.Commit();
        }

        /// <inheritdoc/>
        protected override Task CompleteUowAsync()
        {
            ActiveTransaction?.Commit();

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        protected override void DisposeUow()
        {

        }
    }
}
