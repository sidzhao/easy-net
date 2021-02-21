using System;
using System.Threading;
namespace EasyNet.Data
{
    /// <summary>
    /// CallContext implementation of <see cref="ICurrentDbConnectorProvider"/>. 
    /// This is the default implementation.
    /// </summary>
    public class AsyncLocalCurrentDbConnectorProvider : ICurrentDbConnectorProvider
    {
        /// <summary>
        /// AsyncLocal must be used instead of ThreadLocal, otherwise the value is not maintained in the asynchronous context.
        /// </summary>
        private static readonly AsyncLocal<IDbConnector> AsyncLocalDbConnector = new AsyncLocal<IDbConnector>();

        public IDbConnector Current
        {
            // ReSharper disable once InconsistentlySynchronizedField
            get => AsyncLocalDbConnector.Value;
            set
            {
                lock (AsyncLocalDbConnector)
                {
                    if (AsyncLocalDbConnector.Value == null)
                    {
                        AsyncLocalDbConnector.Value = value;
                    }
                    else
                    {
                        throw new EasyNetException("Already start a database connection.");
                    }
                }
            }
        }
    }
}
