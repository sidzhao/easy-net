using System.Data;

namespace EasyNet.Data
{
    public class DbConnector : IDbConnector
    {
        public IDbConnection Connection { get; set; }

        public IDbTransaction Transaction { get; set; }

        public virtual void Dispose()
        {
            Transaction?.Dispose();
            Connection?.Dispose();
        }
    }
}