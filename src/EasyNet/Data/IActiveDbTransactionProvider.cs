using System.Data;

namespace EasyNet.Data
{
    public interface IActiveDbTransactionProvider
    {
        IDbConnection Connection { get; }

        IDbTransaction Transaction { get; }
    }
}
