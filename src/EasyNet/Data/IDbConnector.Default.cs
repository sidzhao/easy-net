using System.Data;

namespace EasyNet.Data
{
    public class DbConnector : IDbConnector
    {
        public IDbConnection Connection { get; set; }

        public IDbTransaction Transaction { get; set; }

        public virtual void Dispose()
        {
            Connection?.Dispose();
            Connection = null;

            Transaction?.Dispose();
            Transaction = null;
        }
    }
}