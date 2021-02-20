using System.Data;

namespace EasyNet.Data
{
    internal class NullDbTransactionProvider : IActiveDbTransactionProvider
    {
        public IDbConnection Connection { get; }
        public IDbTransaction Transaction { get; }
    }
}
