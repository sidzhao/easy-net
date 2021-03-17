using System;

namespace EasyNet.Data
{
    /// <summary>
    /// Used to get/set current <see cref="IDbConnector"/>. 
    /// </summary>
    public interface ICurrentDbConnectorProvider : IDisposable
    {
        /// <summary>
        /// Gets current <see cref="IDbConnector"/>.
        /// </summary>
        IDbConnector Current { get; }

        /// <summary>
        /// Gets current <see cref="IDbConnector"/>. If it's null then create it directly.
        /// </summary>
        /// <returns></returns>
        IDbConnector GetOrCreate();
    }
}
