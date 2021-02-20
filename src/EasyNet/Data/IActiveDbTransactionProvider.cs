using System.Data;

namespace EasyNet.Data
{
    /// <summary>
    /// It' used to get current database transaction.
    /// </summary>
    public interface IActiveDbTransactionProvider
    {
        /// <summary>
        /// Gets current database connection.
        /// </summary>
        IDbConnection Connection { get; }

        /// <summary>
        /// Gets current database transaction.
        /// </summary>
        IDbTransaction Transaction { get; }
    }
}
