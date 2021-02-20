using System.Data;

namespace EasyNet.Data
{
    /// <summary>
    /// It' used to get current database transaction.
    /// </summary>
    internal class NullDbTransactionProvider : IActiveDbTransactionProvider
    {
        public NullDbTransactionProvider()
        {
            Connection = null;
            Transaction = null;
        }

        public IDbConnection Connection { get;  }

        public IDbTransaction Transaction { get;  }
    }
}
