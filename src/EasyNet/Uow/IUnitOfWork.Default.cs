using System.Threading.Tasks;
using EasyNet.EventBus;
using EasyNet.Runtime.Session;
using Microsoft.Extensions.Options;

namespace EasyNet.Uow
{
    /// <summary>
    /// Null implementation of unit of work.
    /// It's used if no component registered for <see cref="IUnitOfWork"/>.
    /// This ensures working EasyNet without a database.
    /// </summary>
    public sealed class NullUnitOfWork : UnitOfWorkBase
    {
        public NullUnitOfWork(IOptions<UnitOfWorkDefaultOptions> defaultOptions) : base(NullEasyNetSession.Instance, null, defaultOptions)
        {
        }

        public override void SaveChanges()
        {

        }

        /// <inheritdoc/>
        public override Task SaveChangesAsync()
        {
            return Task.FromResult(0);
        }

        /// <inheritdoc/>
        protected override void BeginUow()
        {

        }

        /// <inheritdoc/>
        protected override void CompleteUow()
        {

        }

        /// <inheritdoc/>
        protected override Task CompleteUowAsync()
        {
            return Task.FromResult(0);
        }

        /// <inheritdoc/>
        protected override void DisposeUow()
        {

        }
    }
}
