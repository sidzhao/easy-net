using DotNetCore.CAP;
using DotNetCore.CAP.Transport;

namespace EasyNet.EventBus.Cap.MySql
{
    public class EasyNetMySqlCapTransaction : MySqlCapTransaction
    {
        public EasyNetMySqlCapTransaction(IDispatcher dispatcher) : base(dispatcher)
        {
        }

        public override void Dispose()
        {
            // No need to release the transaction.
        }
    }
}
