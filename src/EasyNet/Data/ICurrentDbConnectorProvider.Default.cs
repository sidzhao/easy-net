using System.Threading;
using EasyNet.Uow;

namespace EasyNet.Data
{
    /// <summary>
    /// CallContext implementation of <see cref="ICurrentDbConnectorProvider"/>. 
    /// This is the default implementation.
    /// </summary>
    public class AsyncLocalCurrentDbConnectorProvider : ICurrentDbConnectorProvider
    {
        protected readonly ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider;
        protected readonly IDbConnectorCreator DbConnectorCreator;

        /// <summary>
        /// AsyncLocal must be used instead of ThreadLocal, otherwise the value is not maintained in the asynchronous context.
        /// </summary>
        private static readonly AsyncLocal<IDbConnector> AsyncLocalDbConnector = new AsyncLocal<IDbConnector>();

        public AsyncLocalCurrentDbConnectorProvider(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IDbConnectorCreator dbConnectorCreator)
        {
            CurrentUnitOfWorkProvider = currentUnitOfWorkProvider;
            DbConnectorCreator = dbConnectorCreator;
        }

        public IDbConnector Current
        {
            get
            {
                if (CurrentUnitOfWorkProvider.Current != null)
                {
                    return CurrentUnitOfWorkProvider.Current.DbConnector;
                }
                else
                {
                    return AsyncLocalDbConnector.Value;
                }
            }
        }

        public IDbConnector GetOrCreate()
        {
            if (Current != null) 
                return Current;

            // If the current unit of work is null, then create DbConnector directly and put into AsyncLocal.
            if (CurrentUnitOfWorkProvider.Current == null)
            {
                var dbConnector = DbConnectorCreator.Create();
                AsyncLocalDbConnector.Value = dbConnector;

                return dbConnector;
            }
            else // If the current unit of work is null, then create DbConnector base on unit of work
            {
                if (CurrentUnitOfWorkProvider.Current is UnitOfWorkBase uow)
                {
                    uow.SetDbConnector(DbConnectorCreator.Create(uow.Options));
                    return uow.DbConnector;
                }

                throw new EasyNetException($"The interface {typeof(IDbConnector).AssemblyQualifiedName} is not implemented with class {typeof(UnitOfWorkBase)}.");
            }
        }
    }
}
